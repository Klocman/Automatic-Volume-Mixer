using System;
using CSCore.CoreAudioAPI;

namespace Avm.Daemon
{
    internal class SessionCreatedNotification : IAudioSessionNotification
    {
        public int OnSessionCreated(IntPtr newSession)
        {
            SessionCreated?.Invoke(this, new SessionCreatedNotificationEventArgs(newSession));
            return 0;
        }

        public event EventHandler<SessionCreatedNotificationEventArgs> SessionCreated;
    }
}