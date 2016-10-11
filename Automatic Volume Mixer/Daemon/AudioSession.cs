using System;
using System.Diagnostics;
using CSCore.CoreAudioAPI;

namespace Avm.Daemon
{
    public sealed class AudioSession
    {
        private AudioMeterInformation _audioInformation;
        private SimpleAudioVolume _audioVolume;
        private AudioSessionControl2 _sessionControl2;

        // Values that don't change
        private bool? _isSingleProcessSession;
        private bool? _isSystemSoundSession;
        private string _processName;
        private int _assignedProcessId;
        private Process _assignedProcess;
        private bool _assignedProcessGotten;

        // Values that can change, can be reset by FlushBufferedValues()
        private float? _masterVolume;
        private float? _peakValue;
        private string _displayName;
        private bool? _isMuted;

        private readonly object _bufferedValuesLock = new object();

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

        public AudioSession(AudioSessionControl sessionControl)
        {
            SessionControl = sessionControl;
        }

        public void RegisterAudioSessionNotification(IAudioSessionEvents notifications)
        {
            AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => SessionControl.RegisterAudioSessionNotification(notifications));
        }

        public void UnregisterAudioSessionNotification(IAudioSessionEvents notifications)
        {
            AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => SessionControl.UnregisterAudioSessionNotification(notifications));
        }

        private AudioSessionControl SessionControl { get; }

        private AudioSessionControl2 SessionControl2 =>
            _sessionControl2 ?? (_sessionControl2 = SessionControl.QueryInterface<AudioSessionControl2>());

        private AudioMeterInformation AudioInformation =>
            _audioInformation ?? (_audioInformation = SessionControl.QueryInterface<AudioMeterInformation>());

        private SimpleAudioVolume AudioVolume =>
            _audioVolume ?? (_audioVolume = SessionControl.QueryInterface<SimpleAudioVolume>());

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
                        AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _peakValue = AudioInformation.PeakValue > 1 || AudioInformation.PeakValue < 0
                        ? 0 : AudioInformation.PeakValue);
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
                        AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _masterVolume = AudioVolume.MasterVolume);
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
                        AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => AudioVolume.MasterVolume = trimmed);
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
                        AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _isMuted = AudioVolume.IsMuted);
                    return _isMuted.Value;
                }
            }
            set
            {
                lock (_bufferedValuesLock)
                {
                    if (IsMuted == value) return;

                    AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => AudioVolume.IsMuted = value);
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
                    AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _isSingleProcessSession = SessionControl2.IsSingleProcessSession);
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
                    AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _isSystemSoundSession = SessionControl2.IsSystemSoundSession);
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
                        AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _displayName = GetDisplayName());
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
                        AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _assignedProcessId = SessionControl2.ProcessID);
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
                            AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _assignedProcess = SessionControl2.Process);
                        }
                    }
                    else
                    {
                        AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _assignedProcess = SessionControl2.Process);
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
                    AudioSessionUpdateThread.Instance.RunSynchronizedAction(() => _processName = AssignedProcess?.ProcessName ?? string.Empty);
                return _processName;
            }
        }

        private string GetDisplayName()
        {
            if (IsSystemSoundSession)
                return "System Sounds";
            string name = null;
            AudioSessionUpdateThread.Instance.RunSynchronizedAction(() =>
            {
                if (!string.IsNullOrWhiteSpace(SessionControl.DisplayName))
                {
                    name = SessionControl.DisplayName;
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
            AudioSessionUpdateThread.Instance.RunSynchronizedAction(() =>
            {
                try
                {
                    result = IsSystemSoundSession || ((!IsSingleProcessSession || (AssignedProcess != null))
                           && SessionControl.SessionState != AudioSessionState.AudioSessionStateExpired);
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