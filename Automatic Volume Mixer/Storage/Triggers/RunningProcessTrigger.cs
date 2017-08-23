using System;
using System.ComponentModel;
using System.Linq;
using Avm.Daemon;

namespace Avm.Storage.Triggers
{
    public class RunningProcessTrigger : NameFilterTriggerBase, ITrigger
    {
        [Category("Process Trigger")]
        [Description("Running will fire when any process matching the criteria is running. OnStart will fire only once when the first matching process starts (not on second, third etc.), while OnClose will fire when the last matching process closes.")]
        [DefaultValue(ProcessTriggerType.Running)]
        public ProcessTriggerType TriggerType { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        private bool? _lastState;

        public bool ProcessTrigger(object sender, StateUpdateEventArgs args)
        {
            var result = MixerWatcher.ProcessBuffer.Any(x => MatchName(x.ProcessName));
            try
            {
                switch (TriggerType)
                {
                    case ProcessTriggerType.Running:
                        return result;
                    case ProcessTriggerType.OnStart:
                        return result && _lastState.HasValue && !_lastState.Value;
                    case ProcessTriggerType.OnClose:
                        return !result && _lastState.HasValue && _lastState.Value;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            finally
            {
                _lastState = result;
            }
        }
    }

    public enum ProcessTriggerType
    {
        Running,
        OnStart,
        OnClose
    }
}