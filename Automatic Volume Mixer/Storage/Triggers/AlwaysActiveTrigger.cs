using System.ComponentModel;
using Avm.Daemon;

namespace Avm.Storage.Triggers
{
    [DefaultProperty(nameof(Enabled))]
    public class AlwaysActiveTrigger : BasicInfoBase
    {
        private uint _triggerStore;

        [Description("How many updates to pass before triggering again. If set to 0 trigger runs every update.")]
        [DefaultValue(0)]
        public uint TriggerEvery { get; set; } = 0;

        public override string GetDetails()
        {
            return "Trigger that fires every update";
        }

        public bool ProcessTrigger(object sender, StateUpdateEventArgs args)
        {
            if (!Enabled) return false;

            if (++_triggerStore <= TriggerEvery) return false;

            _triggerStore = 0;
            return true;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}