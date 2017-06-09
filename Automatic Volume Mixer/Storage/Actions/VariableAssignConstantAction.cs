using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Avm.Daemon;

namespace Avm.Storage.Actions
{
    [DefaultProperty(nameof(VariableName))]
    public class VariableAssignConstantAction : BasicInfoBase, IAction
    {
        [Category("Variable")]
        [Description("Name of the checked variable.")]
        public string VariableName { get; set; } = "New variable";

        [Category("Variable")]
        [Description("Value to set to the variable.")]
        [DefaultValue(1f)]
        public float NewValue { get; set; } = 1f;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            Debug.Assert(Enabled, "Enabled");

            args.VariableStore[VariableName] = NewValue;
        }

        public override string GetDetails()
        {
            return $"Set variable \"{VariableName}\" to {NewValue.ToString(CultureInfo.CurrentCulture)}";
        }
    }
}