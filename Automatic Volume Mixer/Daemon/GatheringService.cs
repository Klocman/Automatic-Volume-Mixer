using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Timers;
using Klocman.Forms.Tools;

namespace Avm.Daemon
{
    public class GatheringService : IDisposable
    {
        private readonly ReplaySubject<StateUpdateEventArgs> _mixerUpdateSubject =
            new ReplaySubject<StateUpdateEventArgs>(1);

        private readonly MixerWatcher _mixerWatcher = new MixerWatcher();
        private readonly Timer _timer;

        public GatheringService()
        {
            _timer = new Timer(500) {AutoReset = false};
            _timer.Elapsed += (sender, args) => SendAudioSessionUpdate(args.SignalTime);
            _timer.Disposed += (sender, args) => _mixerUpdateSubject.OnCompleted();

            // Make sure that the ReplaySubject always has a value
            SendAudioSessionUpdate(DateTime.Now);
            _timer.Start();
        }

        public IObservable<StateUpdateEventArgs> AudioSessionUpdate => _mixerUpdateSubject.AsObservable();

        /// <summary>
        ///     Updates per second
        /// </summary>
        public double RefreshRate
        {
            get { return 1000/_timer.Interval; }
            set { _timer.Interval = 1/value*1000; }
        }

        public IEnumerable<AudioSession> AudioSessions => _mixerUpdateSubject.Take(1).ToEnumerable().First().Sessions;

        public void Dispose()
        {
            _timer.Dispose();
            _mixerUpdateSubject.OnCompleted();
            _mixerWatcher.Dispose();
        }

        private void SendAudioSessionUpdate(DateTime triggerTime)
        {
            try
            {
                _mixerUpdateSubject.OnNext(
                    new StateUpdateEventArgs(_mixerWatcher.GetAudioSessions().ToList().AsReadOnly(), triggerTime));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($@"Exception in {nameof(SendAudioSessionUpdate)} -> {ex.Message}" +
                                (ex.InnerException == null ? string.Empty : $@"-> {ex.InnerException.Message}"));
                PremadeDialogs.GenericError(ex);
                _mixerUpdateSubject.OnError(ex);
            }
            finally
            {
                _timer.Start();
            }
        }
    }
}