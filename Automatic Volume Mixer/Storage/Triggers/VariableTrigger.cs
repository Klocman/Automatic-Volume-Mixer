using System;
using System.ComponentModel;
using System.Globalization;
using Avm.Daemon;

namespace Avm.Storage.Triggers
{
    public class VariableTrigger : BasicInfoBase, ITrigger
    {
        [Category("Variable")]
        [Description("Name of the checked variable.")]
        public string VariableName { get; set; } = "New variable";

        [Category("Variable")]
        [Description("Value to compare against.")]
        [DefaultValue(1f)]
        public float Value { get; set; } = 1f;

        [Category("Variable")]
        [Description("Method of comparison.")]
        [DefaultValue(VariableComparisonType.Equal)]
        public VariableComparisonType ComparisonType { get; set; } = VariableComparisonType.Equal;

        public override string GetDetails()
        {
            return $"Variable \"{VariableName}\" is {ComparisonType} {Value.ToString(CultureInfo.CurrentCulture)}";
        }

        public bool ProcessTrigger(object sender, StateUpdateEventArgs args)
        {
            if (!Enabled) return false;

            float variableValue;
            if (!args.VariableStore.TryGetValue(VariableName, out variableValue))
                return false;

            switch (ComparisonType)
            {
                case VariableComparisonType.Equal:
                    return Math.Abs(variableValue - Value) < 0.000001;
                case VariableComparisonType.LargerThan:
                    return variableValue > Value;
                case VariableComparisonType.LowerThan:
                    return variableValue < Value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}