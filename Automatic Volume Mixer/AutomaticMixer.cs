using System;
using System.Collections.Generic;
using System.Linq;
using Avm.Daemon;
using Avm.Storage;
using Klocman.Forms.Tools;

namespace Avm
{
    public class AutomaticMixer
    {
        private readonly BehaviourManager _behaviourManager;
        //private readonly List<Behaviour> _behaviours = new List<Behaviour>();
        private readonly GatheringService _gatheringService;

        public AutomaticMixer()
        {
            _behaviourManager = new BehaviourManager();
            _gatheringService = new GatheringService();
            _gatheringService.Stop();
            _gatheringService.StateUpdate += OnMixerStateUpdate;
        }

        public bool BehavioursEnabled
        {
            get { return _behaviourManager.Enabled; }
            set
            {
                if (_behaviourManager.Enabled == value) return;

                _behaviourManager.Enabled = value;
                BehavioursEnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        //public IList<Behaviour> Behaviours => _behaviours;
        public IEnumerable<KeyValuePair<int, AudioSession>> Sessions => _gatheringService.AudioSessions;
        public IEnumerable<Behaviour> Behaviours => _behaviourManager.GetBehaviours() ?? Enumerable.Empty<Behaviour>();

        public IEnumerable<string> GroupNamesEnumerable
        {
            get
            {
                return from groupName in Behaviours.Select(x => x.Group).Distinct()
                       where !string.IsNullOrEmpty(groupName)
                       orderby groupName ascending
                       select groupName;
            }
        }

        public event EventHandler BehavioursEnabledChanged;
        public event EventHandler<StateUpdateEventArgs> MixerStateUpdate;

        public void ResetSessionVolumes()
        {
            foreach (var kvp in Sessions)
            {
                kvp.Value.IsMuted = false;
                kvp.Value.MasterVolume = 1;
            }
        }

        public void AddBehaviour(Behaviour item)
        {
            _behaviourManager.AddBehaviour(item);
        }

        public void RemoveBehaviour(Behaviour item)
        {
            _behaviourManager.RemoveBehaviour(item);
        }

        private void OnMixerStateUpdate(object sender, StateUpdateEventArgs e)
        {
            MixerStateUpdate?.Invoke(sender, e);

            _behaviourManager?.ProcessEvents(sender, e);
        }

        public void Start()
        {
            _gatheringService.Start();
        }

        public void Stop()
        {
            _gatheringService.Stop();
        }

        public string GetBehavioursAsString(bool disableFormatting)
        {
            return _behaviourManager.SerializeBehaviours(disableFormatting);
        }

        public event EventHandler BehavioursChanged
        {
            add { _behaviourManager.BehavioursChanged += value; }
            remove { _behaviourManager.BehavioursChanged -= value; }
        }

        public void SetBehavioursFromString(string behaviours, bool clearExisting = true)
        {
            _behaviourManager.DeserializeBehaviours(behaviours, clearExisting);
        }
    }
}