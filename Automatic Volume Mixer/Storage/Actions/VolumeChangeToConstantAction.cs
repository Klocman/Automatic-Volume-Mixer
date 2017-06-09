using System;
using System.ComponentModel;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    [DefaultProperty(nameof(NewVolume))]
    public class VolumeChangeToConstantAction : VolumeChangeActionBase, IAction
    {
        private float _newVolume = 0.5f;

        [Category("Volume change")]
        [Description("New volume the applications should be set to.")]
        [DefaultValue(0.5f)]
        public float NewVolume
        {
            get { return _newVolume; }
            set { _newVolume = Math.Max(Math.Min(value, 1f), 0f); }
        }

        public override string GetDetails()
        {
            return $@"Set volume to {NewVolume} over {SecondsToChange}s; {base.GetDetails()}";
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            Run(args, _newVolume);
        }
    }
}