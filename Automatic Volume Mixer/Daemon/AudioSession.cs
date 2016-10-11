using System;
using System.Diagnostics;
using CSCore.CoreAudioAPI;

namespace Avm.Daemon
{
    public sealed class AudioSession
    {
        private Process _assignedProcess;
        private bool _assignedProcessGotten;
        private AudioMeterInformation _audioInformation;
        private SimpleAudioVolume _audioVolume;
        private string _displayName;
        private bool? _isMuted;
        private bool? _isSingleProcessSession;
        // No need to ever recheck this
        private bool? _isSystemSoundSession;
        private float? _masterVolume;
        private float? _peakValue;
        private string _processName;
        private AudioSessionControl2 _sessionControl2;

        public AudioSession(AudioSessionControl sessionControl)
        {
            SessionControl = sessionControl;
        }

        public AudioSessionControl SessionControl { get; }

        public AudioSessionControl2 SessionControl2 =>
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
                if (!_peakValue.HasValue)
                    _peakValue = AudioInformation.PeakValue > 1 || AudioInformation.PeakValue < 0
                        ? 0
                        : AudioInformation.PeakValue;
                return _peakValue.Value;
            }
        }

        public float MasterVolume
        {
            get
            {
                if (!_masterVolume.HasValue)
                    _masterVolume = AudioVolume.MasterVolume;
                return _masterVolume.Value;
            }
            set
            {
                var trimmed = Math.Max(Math.Min(value, 1f), 0f);
                if (!_masterVolume.HasValue || Math.Abs(trimmed - _masterVolume.Value) > 0.0001f)
                {
                    _masterVolume = trimmed;
                    AudioVolume.MasterVolume = trimmed;
                }
            }
        }

        public bool IsMuted
        {
            get
            {
                if (!_isMuted.HasValue)
                    _isMuted = AudioVolume.IsMuted;
                return _isMuted.Value;
            }
            set
            {
                if (IsMuted == value) return;

                _isMuted = value;
                AudioVolume.IsMuted = value;
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
                    _isSingleProcessSession = SessionControl2.IsSingleProcessSession;
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
                    _isSystemSoundSession = SessionControl2.IsSystemSoundSession;
                return _isSystemSoundSession.Value;
            }
        }

        /// <summary>
        ///     Display mame of this session. Can be a specially set session name, title of the owner's main window.
        ///     If all else fails, the SessionIdentifier is returned. Return value is buffered.
        /// </summary>
        public string DisplayName => _displayName ?? (_displayName = GetDisplayName());

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
                    _assignedProcessGotten = true;
                    _assignedProcess = SessionControl2.Process;
                }
                else if (_assignedProcess != null && _assignedProcess.HasExited)
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
                    _processName = AssignedProcess?.ProcessName ?? string.Empty;
                return _processName;
            }
        }

        /// <summary>
        ///     Clears buffers to force some of the more critical values to be re-evaluated upon next request
        /// </summary>
        public void FlushBufferedValues()
        {
            _displayName = null;
            _peakValue = null;
            _masterVolume = null;
            _isMuted = null;
        }

        private string GetDisplayName()
        {
            if (IsSystemSoundSession)
                return "System Sounds";

            if (!string.IsNullOrWhiteSpace(SessionControl.DisplayName))
                return SessionControl.DisplayName;

            //if (AssignedProcess != null)
            {
                var prn = ProcessName;
                if (!string.IsNullOrWhiteSpace(prn))
                    return prn;

                //if (!string.IsNullOrWhiteSpace(process.MainWindowTitle))
                //    return process.MainWindowTitle;
            }

            return SessionControl2.SessionIdentifier;
        }
    }
}