using System;
using System.Diagnostics;
using CSCore.CoreAudioAPI;

namespace Avm.Daemon
{
    public sealed class AudioSession
    {
        private readonly AudioSessionUpdateThread _audioSessionUpdateThread;

        private readonly object _bufferedValuesLock = new object();
        private readonly AudioSessionControl _sessionControl;
        private Process _assignedProcess;
        private bool _assignedProcessGotten;
        private int _assignedProcessId;

        // Values that don't change once set
        private AudioMeterInformation _audioInformation;
        private SimpleAudioVolume _audioVolume;
        private string _displayName;
        private bool? _isMuted;
        private bool? _isSingleProcessSession;
        private bool? _isSystemSoundSession;

        // Values that can change, can be reset by FlushBufferedValues()
        private float? _masterVolume;
        private float? _peakValue;
        private string _processName;
        private AudioSessionControl2 _sessionControl2;

        public AudioSession(AudioSessionControl sessionControl, AudioSessionUpdateThread updateThread)
        {
            _sessionControl = sessionControl;
            _audioSessionUpdateThread = updateThread;
        }

        private AudioSessionControl2 SessionControl2 =>
            _sessionControl2 ?? (_sessionControl2 = _sessionControl.QueryInterface<AudioSessionControl2>());

        private AudioMeterInformation AudioInformation =>
            _audioInformation ?? (_audioInformation = _sessionControl.QueryInterface<AudioMeterInformation>());

        private SimpleAudioVolume AudioVolume =>
            _audioVolume ?? (_audioVolume = _sessionControl.QueryInterface<SimpleAudioVolume>());

        /// <summary>
        ///     Peak value of the application's sound output at this moment
        /// </summary>
        public float PeakValue
        {
            get
            {
                lock (_bufferedValuesLock)
                {
                    if (!_peakValue.HasValue)
                        _audioSessionUpdateThread.RunSynchronizedAction(
                            () => _peakValue = AudioInformation.PeakValue > 1 || AudioInformation.PeakValue < 0
                                ? 0
                                : AudioInformation.PeakValue);
                    return _peakValue.Value;
                }
            }
        }

        public float MasterVolume
        {
            get
            {
                lock (_bufferedValuesLock)
                {
                    if (!_masterVolume.HasValue)
                        _audioSessionUpdateThread.RunSynchronizedAction(() => _masterVolume = AudioVolume.MasterVolume);
                    return _masterVolume.Value;
                }
            }
            set
            {
                lock (_bufferedValuesLock)
                {
                    var trimmed = Math.Max(Math.Min(value, 1f), 0f);
                    if (!_masterVolume.HasValue || Math.Abs(trimmed - _masterVolume.Value) > 0.0001f)
                    {
                        _audioSessionUpdateThread.RunSynchronizedAction(() => AudioVolume.MasterVolume = trimmed);
                        _masterVolume = trimmed;
                    }
                }
            }
        }

        public bool IsMuted
        {
            get
            {
                lock (_bufferedValuesLock)
                {
                    if (!_isMuted.HasValue)
                        _audioSessionUpdateThread.RunSynchronizedAction(() => _isMuted = AudioVolume.IsMuted);
                    return _isMuted.Value;
                }
            }
            set
            {
                lock (_bufferedValuesLock)
                {
                    if (IsMuted == value) return;

                    _audioSessionUpdateThread.RunSynchronizedAction(() => AudioVolume.IsMuted = value);
                    _isMuted = value;
                }
            }
        }

        /// <summary>
        ///     Whether the session has multiple processes assigned to it. Buffered once since it will not change.
        /// </summary>
        public bool IsSingleProcessSession
        {
            get
            {
                if (!_isSingleProcessSession.HasValue)
                    _audioSessionUpdateThread.RunSynchronizedAction(
                        () => _isSingleProcessSession = SessionControl2.IsSingleProcessSession);
                return _isSingleProcessSession.Value;
            }
        }

        /// <summary>
        ///     Whether the session is used for system sounds. Buffered once since it will not change.
        /// </summary>
        public bool IsSystemSoundSession
        {
            get
            {
                if (!_isSystemSoundSession.HasValue)
                    _audioSessionUpdateThread.RunSynchronizedAction(
                        () => _isSystemSoundSession = SessionControl2.IsSystemSoundSession);
                return _isSystemSoundSession.Value;
            }
        }

        /// <summary>
        ///     Display mame of this session. Can be a specially set session name, title of the owner's main window.
        ///     If all else fails, the SessionIdentifier is returned. Return value is buffered.
        /// </summary>
        public string DisplayName
        {
            get
            {
                lock (_bufferedValuesLock)
                {
                    if (_displayName == null)
                        _audioSessionUpdateThread.RunSynchronizedAction(() => _displayName = GetDisplayName());
                    return _displayName;
                }
            }
        }

        public int AssignedProcessId
        {
            get
            {
                if (_assignedProcessId <= 0)
                {
                    if (_assignedProcess != null)
                        _assignedProcessId = _assignedProcess.Id;
                    else
                        _audioSessionUpdateThread.RunSynchronizedAction(
                            () => _assignedProcessId = SessionControl2.ProcessID);
                }
                return _assignedProcessId;
            }
        }

        /// <summary>
        ///     Process assigned to this audio session. In case of multi-process sessions the starting process is returned.
        ///     The process is gotten only once since it will not change. If the process has already exited, null is returned.
        /// </summary>
        public Process AssignedProcess
        {
            get
            {
                if (!_assignedProcessGotten)
                {
                    if (_assignedProcessId > 0)
                    {
                        try
                        {
                            _assignedProcess = Process.GetProcessById(_assignedProcessId);
                        }
                        catch (ArgumentException)
                        {
                            _audioSessionUpdateThread.RunSynchronizedAction(
                                () => _assignedProcess = SessionControl2.Process);
                        }
                    }
                    else
                    {
                        _audioSessionUpdateThread.RunSynchronizedAction(() => _assignedProcess = SessionControl2.Process);
                    }
                    _assignedProcessGotten = true;
                }
                else if (_assignedProcess != null && _assignedProcessId != 0 && _assignedProcess.HasExited)
                    _assignedProcess = null;

                return _assignedProcess;
            }
        }

        /// <summary>
        ///     Name of the assigned process. It is gotten only once since it can't be changed.
        /// </summary>
        public string ProcessName
        {
            get
            {
                if (_processName == null)
                    _audioSessionUpdateThread.RunSynchronizedAction(
                        () => _processName = AssignedProcess?.ProcessName ?? string.Empty);
                return _processName;
            }
        }

        /// <summary>
        ///     Clears buffers to force some of the more critical values to be re-evaluated upon next request
        /// </summary>
        public void FlushBufferedValues()
        {
            lock (_bufferedValuesLock)
            {
                _displayName = null;
                _peakValue = null;
                _masterVolume = null;
                _isMuted = null;
            }
        }

        public void RegisterAudioSessionNotification(IAudioSessionEvents notifications)
        {
            _audioSessionUpdateThread.RunSynchronizedAction(
                () => _sessionControl.RegisterAudioSessionNotification(notifications));
        }

        public void UnregisterAudioSessionNotification(IAudioSessionEvents notifications)
        {
            _audioSessionUpdateThread.RunSynchronizedAction(
                () => _sessionControl.UnregisterAudioSessionNotification(notifications));
        }

        private string GetDisplayName()
        {
            if (IsSystemSoundSession)
                return "System Sounds";
            string name = null;
            _audioSessionUpdateThread.RunSynchronizedAction(() =>
            {
                if (!string.IsNullOrWhiteSpace(_sessionControl.DisplayName))
                {
                    name = _sessionControl.DisplayName;
                    return;
                }

                var prn = ProcessName;
                if (!string.IsNullOrWhiteSpace(prn))
                {
                    name = prn;
                    return;
                }

                name = SessionControl2.SessionIdentifier;
            });
            return name;
        }

        public bool CheckSessionIsValid()
        {
            var result = false;
            _audioSessionUpdateThread.RunSynchronizedAction(() =>
            {
                try
                {
                    result = IsSystemSoundSession || ((!IsSingleProcessSession || (AssignedProcess != null))
                                                      &&
                                                      _sessionControl.SessionState !=
                                                      AudioSessionState.AudioSessionStateExpired);
                }
                catch
                {
                    result = false;
                }
            });
            return result;
        }
    }
}