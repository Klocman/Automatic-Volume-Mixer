using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    [DefaultProperty(nameof(SecondsToWait))]
    public class DelayAction : BasicInfoBase, IAction
    {
        private double _secondsToWait = 5;

        [Category("Delay")]
        [Description("Time in seconds to wait for. This blocks execution.")]
        [DefaultValue(5)]
        public double SecondsToWait
        {
            get { return _secondsToWait; }
            set { _secondsToWait = Math.Max(value, 0f); }
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            Debug.Assert(Enabled, "Enabled");

            Task.Delay(TimeSpan.FromSeconds(SecondsToWait)).Wait();
        }

        public override string GetDetails()
        {
            return $@"Wait for {SecondsToWait}s";
        }
    }
}