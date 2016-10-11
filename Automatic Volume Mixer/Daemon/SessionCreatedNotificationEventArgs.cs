using System;

namespace Avm.Daemon
{
    internal class SessionCreatedNotificationEventArgs : EventArgs
    {
        public SessionCreatedNotificationEventArgs(IntPtr sessionPtr)
        {
            SessionPtr = sessionPtr;
        }

        public IntPtr SessionPtr { get; }
    }
}