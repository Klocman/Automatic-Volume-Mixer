using Avm.Daemon;

namespace Avm.Storage.Triggers
{
    public class RunningSessionTrigger : NameFilterBase, ITrigger
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