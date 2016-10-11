using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using CSCore.CoreAudioAPI;
using Timer = System.Timers.Timer;

namespace Avm.Daemon
{
    public class MixerWatcher : IDisposable
    {
        private readonly List<KeyValuePair<AudioSession, AudioSessionEvents>> _audioSessions;
        private readonly AudioSessionNotification _sessionCreatedNotification;
        private readonly Timer _sessionsChangedTimer;
        private readonly Timer _sessionScrubberTimer;
        private AudioSessionEnumerator _currentSessionEnumerator;
        private AudioSessionManager2 _currentSessionManager;
        private bool _reloadNeeded;
        private bool _reloadQueued;

        public MixerWatcher()
        {
            _sessionsChangedTimer = new Timer(350) {AutoReset = false};
            _sessionsChangedTimer.Elapsed += SessionsChangedTimerElapsed;

            _sessionScrubberTimer = new Timer(650) {AutoReset = true};
            _sessionScrubberTimer.Elapsed += OnSessionScrubbing;

            _sessionCreatedNotification = new AudioSessionNotification(); //= new SessionCreatedNotification();
            _sessionCreatedNotification.SessionCreated += OnSessionCreated; // += OnSessionCreated;

            _audioSessions = new List<KeyValuePair<AudioSession, AudioSessionEvents>>();
            // Queue reload on the next request
            _reloadQueued = true;
        }

        public void Dispose()
        {
            lock (_audioSessions)
            {
                _sessionScrubberTimer.Stop();
                _sessionsChangedTimer.Stop();

                _reloadQueued = false;
                _reloadNeeded = false;

                foreach (var audioSession in _audioSessions)
                {
                    try
                    {
                        audioSession.Key.SessionControl.UnregisterAudioSessionNotification(audioSession.Value);
                    }
                    catch
                    {
                        // Ignore errors, the com object is gone.
                    }
                }
                _audioSessions.Clear();

                _currentSessionEnumerator?.Dispose();
                if (_currentSessionManager != null)
                {
                    _currentSessionManager.UnregisterSessionNotification(_sessionCreatedNotification);
                    _currentSessionManager.Dispose();
                }
            }
        }

        private void OnSessionScrubbing(object sender, ElapsedEventArgs e)
        {
            lock (_audioSessions)
            {
                var itemsRemoved = false;

                foreach (var kvp in _audioSessions.Where(kvp => !CheckSessionIsValid(kvp.Key)).ToList())
                {
                    try
                    {
                        kvp.Key.SessionControl.UnregisterAudioSessionNotification(kvp.Value);
                    }
                    catch (Exception ex)
                    {
                        DebugTools.WriteException(ex);
                    }

                    _audioSessions.Remove(kvp);
                    itemsRemoved = true;
                }

                if (itemsRemoved)
                    FireSessionsChanged(false);
            }
        }

        /// <summary>
        ///     Fired when available audio sessions change (get removed or added).
        /// </summary>
        public event EventHandler AudioSessionsChanged;

        public AudioSession[] GetAudioSessions()
        {
            lock (_audioSessions)
            {
                if (_reloadQueued)
                {
                    _reloadQueued = false;
                    Reload();
                }
                return _audioSessions.Select(x => x.Key).ToArray();
            }
        }
        
        private void OnSessionClosed(object sender, EventArgs e)
        {
            FireSessionsChanged(true);
        }

        private void FireSessionsChanged(bool reenumerationNeeded)
        {
            _sessionScrubberTimer.Stop();
            _sessionsChangedTimer.Stop();

            if (reenumerationNeeded)
                _reloadNeeded = true;

            _sessionsChangedTimer.Start();
        }

        private void SessionsChangedTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _sessionScrubberTimer.Start();

            if (_reloadNeeded)
            {
                _reloadQueued = true;
                _reloadNeeded = false;
            }

            AudioSessionsChanged?.Invoke(this, e);
        }

        private void OnSessionCreated(object sender, SessionCreatedEventArgs e)
        {
            FireSessionsChanged(true);
        }

        private void Reload()
        {
            Dispose();

            // Need to run this in a MTA thread because of RegisterSessionNotification
            var initThread = new Thread(() =>
            {
                _currentSessionManager = GetDefaultAudioSessionManager2(DataFlow.Render);
                _currentSessionEnumerator = _currentSessionManager.GetSessionEnumerator();

                // Possible race condition? Must be ran right before the enumeration
                _currentSessionManager.RegisterSessionNotification(_sessionCreatedNotification);

                _audioSessions.Clear();
                _audioSessions.AddRange(_currentSessionEnumerator
                    //.Select(control => new AudioSessionControl2(control.BasePtr))
                    .Select(x => new AudioSession(x))
                    .Where(CheckSessionIsValid)
                    .Select(x => new KeyValuePair<AudioSession, AudioSessionEvents>(x, AttachEvents(x))));
            })
            {
                IsBackground = false,
                Name = nameof(MixerWatcher) + "_" + nameof(Reload)
            };

            initThread.Start();
            initThread.Join();
            _sessionScrubberTimer.Start();
        }

        private static bool CheckSessionIsValid(AudioSession x)
        {
            try
            {
                return (!x.IsSingleProcessSession || (x.AssignedProcess != null))
                    && x.SessionControl.SessionState != AudioSessionState.AudioSessionStateExpired;
            }
            catch
            {
                return false;
            }
        }

        private AudioSessionEvents AttachEvents(AudioSession sc)
        {
            var ase = new AudioSessionEvents();
            ase.SessionDisconnected += OnSessionClosed;
            ase.StateChanged += (sender, args) =>
            {
                if (!CheckSessionIsValid(sc))
                    OnSessionClosed(sender, args);
            };
            sc.SessionControl.RegisterAudioSessionNotification(ase);
            return ase;
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

/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CSCore.CoreAudioAPI;

namespace Avm.Daemon
{
    public class MixerWatcher : IDisposable
    {
        private readonly List<AudioSessionControl2> _audioSessions;
        private readonly AudioSessionNotification _sessionCreatedNotification;
        private AudioSessionEnumerator _currentSessionEnumerator;
        private AudioSessionManager2 _currentSessionManager;

        public MixerWatcher()
        {
            _sessionCreatedNotification = new AudioSessionNotification(); //= new SessionCreatedNotification();
            _sessionCreatedNotification.SessionCreated += OnSessionCreated;// += OnSessionCreated;

            _audioSessions = new List<AudioSessionControl2>();
            
            new Thread(Reload) { IsBackground = false}.Start();
            //Reload();
        }

        public AudioSessionControl2[] GetAudioSessions()
        {
            lock (_audioSessions) return _audioSessions.ToArray();
        }

        public void Dispose()
        {
            _currentSessionEnumerator?.Dispose();
            if (_currentSessionManager != null)
            {
                _currentSessionManager.UnregisterSessionNotification(_sessionCreatedNotification);
                _currentSessionManager.Dispose();
            }
        }

        private void OnSessionCreated(object sender, SessionCreatedEventArgs e)
        {
            var item = UpgradeSessionControl(e.NewSession);

            lock (_audioSessions)
            {
                _audioSessions.Add(item);
            }
        }

        private void Reload()
        {
            lock (_audioSessions)
            {
                Dispose();

                _currentSessionManager = GetDefaultAudioSessionManager2(DataFlow.Render);
                _currentSessionEnumerator = _currentSessionManager.GetSessionEnumerator();

                // Possible race condition? Must be ran right before the enumeration
                _currentSessionManager.RegisterSessionNotification(_sessionCreatedNotification);

                _audioSessions.Clear();
                _audioSessions.AddRange(_currentSessionEnumerator.Select(UpgradeSessionControl));
            }
        }

        private AudioSessionControl2 UpgradeSessionControl(AudioSessionControl control)
        {
            var item = new AudioSessionControl2(control.BasePtr);
            var eventHandler = new AudioSessionEvents();
            eventHandler.StateChanged +=
                (o, args) =>
                {
                    if (args.NewState == AudioSessionState.AudioSessionStateExpired)
                        lock (_audioSessions) _audioSessions.Remove(item);
                        item.UnregisterAudioSessionNotification(eventHandler);
                };
            item.RegisterAudioSessionNotification(eventHandler);

            return item;
        }

        static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
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
}*/