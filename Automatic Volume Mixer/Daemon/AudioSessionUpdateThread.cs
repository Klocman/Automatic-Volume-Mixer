using System;
using System.Threading;

namespace Avm.Daemon
{
    public class AudioSessionUpdateThread : IDisposable
    {
        private readonly object _actionLock = new object();
        private readonly AutoResetEvent _reset = new AutoResetEvent(false);
        private readonly AutoResetEvent _reset2 = new AutoResetEvent(false);
        private readonly Thread _workerThread;

        private Action _currentAction;

        public AudioSessionUpdateThread()
        {
            _workerThread = new Thread(Thread)
            {
                IsBackground = false,
                Name = nameof(AudioSessionUpdateThread)
            };
            _workerThread.Start();
        }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Disposed = true;
            _reset.Set();
        }

        private void Thread()
        {
            while (true)
            {
                _reset.WaitOne();
                if (Disposed) return;

                _currentAction();
                _reset2.Set();
            }
        }

        /// <summary>
        ///     Runs the supplied action on the AudioSessionUpdateThread and waits until it is done before returning.
        /// </summary>
        public void RunSynchronizedAction(Action a)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(AudioSessionUpdateThread));

            if (System.Threading.Thread.CurrentThread.Equals(_workerThread))
            {
                // Already running on the correct thread
                a();
            }
            else
            {
                lock (_actionLock)
                {
                    _currentAction = a;
                    _reset.Set();

                    // Block until the action is completed by the thread
                    _reset2.WaitOne();
                }
            }
        }
    }
}