using Avm.Daemon;

namespace Avm.Storage.Triggers
{
    public class RunningSessionTrigger : NameFilterTriggerBase, ITrigger
    {
        public virtual bool ProcessTrigger(object sender, StateUpdateEventArgs args)
        {
            return Enabled && MatchUpdate(args);
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}