using System;
using System.Collections.Generic;

namespace Avm.Daemon
{
    public sealed class StateUpdateEventArgs : EventArgs
    {
        public StateUpdateEventArgs(IEnumerable<AudioSession> sessions, DateTime snapshotTime, IDictionary<string, float> variableStore)
        {
            Sessions = sessions;
            SnapshotTime = snapshotTime;
            VariableStore = variableStore;
        }

        /// <summary>
        ///     Time at which the update was sent
        /// </summary>
        public DateTime SnapshotTime { get; }

        /// <summary>
        ///     Read only list of sessions present at the time of snapshot creation.
        /// </summary>
        public IEnumerable<AudioSession> Sessions { get; }

        public IDictionary<string, float> VariableStore { get; }
    }
}