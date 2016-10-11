using Avm.Daemon;

namespace Avm.Storage
{
    public interface IAction : IBasicInfo
    {
        void ExecuteAction(object sender, StateUpdateEventArgs args);
    }
}