using System.Linq;
using Avm.Daemon;

namespace Avm.Storage.Triggers
{
    public class RunningProcessTrigger : NameFilterBase, ITrigger
    {
        public override object Clone()
        {
            return MemberwiseClone();
        }

        public bool ProcessTrigger(object sender, StateUpdateEventArgs args)
        {
            return Enabled && MixerWatcher.ProcessBuffer.Any(x => MatchName(x.ProcessName));
        }
    }
}