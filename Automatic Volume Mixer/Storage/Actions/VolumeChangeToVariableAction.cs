using System;
using System.ComponentModel;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    [DefaultProperty(nameof(VariableName))]
    public class VolumeChangeToVariableAction : VolumeChangeActionBase, IAction
    {
        [Category("Variable")]
        [Description("Name of the checked variable. If the variable doesn't exist, do nothing. Value is clamped between 0 and 1.")]
        public string VariableName { get; set; } = "New variable";

        public override string GetDetails()
        {
            return $"Set volume to value of \"{VariableName}\" over {SecondsToChange}s; {base.GetDetails()}";
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            float newVolume;
            if (args.VariableStore.TryGetValue(VariableName, out newVolume))
                Run(args, Math.Max(Math.Min(newVolume, 1f), 0f));
        }
    }
}