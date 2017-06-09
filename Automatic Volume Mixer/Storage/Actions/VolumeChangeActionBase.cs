using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    public abstract class VolumeChangeActionBase : NameFilterBase
    {
        private readonly TimeSpan _volumeChangeDelay = TimeSpan.FromMilliseconds(150);
        private double _secondsToChange;

        [Category("Volume change")]
        [Description("Time in seconds during which the volume should change. " +
                     "The change is linear from start to end, 0 is instant. This blocks execution.")]
        [DefaultValue(0f)]
        public double SecondsToChange
        {
            get { return _secondsToChange; }
            set { _secondsToChange = Math.Max(value, 0f); }
        }

        protected void Run(StateUpdateEventArgs args, float newVolume)
        {
            Debug.Assert(Enabled, "Enabled");

            var targets = args.Sessions.Where(MatchSessionName)
                .Select(x => new KeyValuePair<AudioSession, float>(x, x.MasterVolume)).ToList();

            if (!targets.Any()) return;

            Action<Func<float, float>> setVolumes = volumeGetter =>
            {
                foreach (var kvp in targets)
                {
                    kvp.Key.MasterVolume = volumeGetter(kvp.Value);
                }
            };

            if (SecondsToChange > 0)
            {
                var start = args.SnapshotTime.Ticks;
                var end = start + TimeSpan.FromSeconds(SecondsToChange).Ticks;
                float endMinusStart = end - start;

                for (var now = DateTime.Now.Ticks; now < end; now = DateTime.Now.Ticks)
                {
                    var timePercentage = (now - start) / endMinusStart;

                    setVolumes(originalVolume => timePercentage * (newVolume - originalVolume) + originalVolume);

                    Task.Delay(_volumeChangeDelay).Wait();
                }
            }

            setVolumes(_ => newVolume);
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}