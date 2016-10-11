using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    [DefaultProperty(nameof(NewVolume))]
    public class VolumeChangeAction : NameFilterBase, IAction
    {
        private readonly TimeSpan _volumeChangeDelay = TimeSpan.FromMilliseconds(150);
        private float _newVolume = 0.5f;
        private double _secondsToChange;

        [Category("Volume change")]
        [Description("New volume the applications should be set to.")]
        [DefaultValue(0.5f)]
        public float NewVolume
        {
            get { return _newVolume; }
            set { _newVolume = Math.Max(Math.Min(value, 1f), 0f); }
        }

        [Category("Volume change")]
        [Description("Time in seconds during which the volume should change. " +
                     "The change is linear from start to end, 0 is instant. This blocks execution.")]
        [DefaultValue(0)]
        public double SecondsToChange
        {
            get { return _secondsToChange; }
            set { _secondsToChange = Math.Max(value, 0f); }
        }

        public override string GetDetails()
        {
            return $@"Set volume to {NewVolume} over {SecondsToChange}s; {base.GetDetails()}";
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            if (!Enabled) return;

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
                    var timePercentage = (now - start)/endMinusStart;

                    setVolumes(originalVolume => timePercentage*(_newVolume - originalVolume) + originalVolume);

                    Task.Delay(_volumeChangeDelay).Wait();
                }
            }

            setVolumes(_ => _newVolume);
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}