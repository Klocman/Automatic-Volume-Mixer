using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Klocman.Extensions;
using Klocman.Forms.Tools;

namespace Avm.Daemon
{
    public class GatheringService
    {
        private readonly MixerWatcher _mixerWatcher = new MixerWatcher();
        private readonly Dictionary<int, AudioSession> _sessions;
        private readonly Timer _timer;
        private bool _running;

        public GatheringService()
        {
            _timer = new Timer(500) {AutoReset = false};
            _timer.Elapsed += StateUpdateTimerElapsed;

            _sessions = new Dictionary<int, AudioSession>();
            ReloadSessions();
            _mixerWatcher.AudioSessionsChanged += OnAudioSessionsChanged;
        }

        /// <summary>
        ///     Get a snapshot of AudioSessions
        /// </summary>
        public IEnumerable<KeyValuePair<int, AudioSession>> AudioSessions
        {
            get
            {
                lock (_sessions)
                {
                    return _sessions.ToList();
                }
            }
        }

        /// <summary>
        ///     Updates per second
        /// </summary>
        public double RefreshRate
        {
            get { return 1000/_timer.Interval; }
            set { _timer.Interval = (1/value)*1000; }
        }

        private void OnAudioSessionsChanged(object sender, EventArgs eventArgs)
        {
            ReloadSessions();
            AudioSessionsChanged?.Invoke(sender, eventArgs);
        }

        public event EventHandler AudioSessionsChanged;

        private void ReloadSessions()
        {
            // Make sure the update is not running while the session list changes (stuff gets disposed)
            lock (_sessions)
            {
                _sessions.Clear();
                foreach (var audioSession in _mixerWatcher.GetAudioSessions())
                {
                    _sessions.Add(audioSession.SessionControl2.ProcessID, audioSession);
                }
            }
        }

        public void Start()
        {
            _running = true;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _running = false;
        }

        public event EventHandler<StateUpdateEventArgs> StateUpdate;

        private void StateUpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_sessions)
            {
                try
                {
                    foreach (var session in _sessions)
                    {
                        session.Value.FlushBufferedValues();
                    }

                    StateUpdate?.Invoke(this, new StateUpdateEventArgs(_sessions.AsReadOnly(), DateTime.Now));
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                    Debug.WriteLine($@"Exception in {nameof(StateUpdateTimerElapsed)} -> {ex.Message}" +
                                    (ex.InnerException == null ? string.Empty : $@"-> {ex.InnerException.Message}"));
                }
                finally
                {
                    if (_running)
                        _timer.Start();
                }
            }
        }
    }
}