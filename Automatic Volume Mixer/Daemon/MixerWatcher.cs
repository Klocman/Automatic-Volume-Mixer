using System;
using System.Collections.Generic;
using System.Linq;
using CSCore.CoreAudioAPI;

namespace Avm.Daemon
{
    public class MixerWatcher : IDisposable
    {
        private readonly AudioSessionUpdateThread _audioSessionUpdateThread = new AudioSessionUpdateThread();
        private readonly object _disposeLock = new object();

        private AudioSessionEnumerator _currentSessionEnumerator;
        private AudioSessionManager2 _currentSessionManager;

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (Disposed)
                return;

            lock (_disposeLock)
            {
                Disposed = true;

                _currentSessionEnumerator?.Dispose();
                _currentSessionManager?.Dispose();
                _audioSessionUpdateThread.Dispose();
            }
        }

        public AudioSession[] GetAudioSessions()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MixerWatcher));

            lock (_disposeLock)
            {
                // Need to be disposed before enumerating again. The order matters.
                _currentSessionEnumerator?.Dispose();
                _currentSessionManager?.Dispose();

                var results = new List<AudioSession>();

                _audioSessionUpdateThread.RunSynchronizedAction(() =>
                {
                    _currentSessionManager = GetDefaultAudioSessionManager2(DataFlow.Render);
                    _currentSessionEnumerator = _currentSessionManager.GetSessionEnumerator();

                    results.AddRange(_currentSessionEnumerator
                        .Select(x => new AudioSession(x, _audioSessionUpdateThread))
                        .Where(x => x.CheckSessionIsValid()));
                });

                return results.ToArray();
            }
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }
    }
}