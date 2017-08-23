using Avm.Daemon;

namespace Avm.Storage
{
    public interface ITrigger : IBasicInfo
    {
        bool ProcessTrigger(object sender, StateUpdateEventArgs args);

        bool InvertResult { get; set; }
    }
}