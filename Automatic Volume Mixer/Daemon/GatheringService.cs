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
        private readonly Timer _timer;
        private bool _running;

        public GatheringService()
        {
            _timer = new Timer(500) { AutoReset = false };
            _timer.Elapsed += StateUpdateTimerElapsed;
            
            //_mixerWatcher.AudioSessionsChanged += OnAudioSessionsChanged;
        }

        /// <summary>
        ///     Updates per second
        /// </summary>
        public double RefreshRate
        {
            get { return 1000 / _timer.Interval; }
            set { _timer.Interval = (1 / value) * 1000; }
        }

        //TODO not assigned
        public IEnumerable<KeyValuePair<int, AudioSession>> AudioSessions { get; private set; }

        private void OnAudioSessionsChanged(object sender, EventArgs eventArgs)
        {
            AudioSessionsChanged?.Invoke(sender, eventArgs);
        }

        public event EventHandler AudioSessionsChanged;

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
            //TODO BUG Don't re-get the sessions every single time? Less efficient, but makes lots of code unnecessary
            var sessionstemp = _mixerWatcher.GetAudioSessions().ToList();

            try
            {
                foreach (var session in sessionstemp)
                {
                    session.FlushBufferedValues();
                }

                StateUpdate?.Invoke(this, new StateUpdateEventArgs(sessionstemp.AsReadOnly(), DateTime.Now));
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