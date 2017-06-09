using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avm.Daemon;
using Avm.Storage.Triggers;

namespace Avm.Storage.Actions
{
    [DefaultProperty(nameof(VariableName))]
    public class VariableAssignVolumeAction : NameFilterBase, IAction
    {
        [Category("Variable")]
        [Description("Name of the checked variable.")]
        public string VariableName { get; set; } = "New variable";

        [Category("Variable")]
        [Description("Value to set to the variable.")]
        [DefaultValue(VolumeType.Peak)]
        public VolumeType TargetValue { get; set; } = VolumeType.Peak;

        [Category("Variable")]
        [Description("What to do if there are multiple volume values. If there are no values, nothing is assigned.")]
        [DefaultValue(ValueSelectionMethod.Average)]
        public ValueSelectionMethod ValueSelectionMethod { get; set; } = ValueSelectionMethod.Average;

        public override object Clone()
        {
            return MemberwiseClone();
        }

        protected float GetValueFromSession(AudioSession session)
        {
            switch (TargetValue)
            {
                case VolumeType.Peak:
                    return session.PeakValue;
                case VolumeType.Master:
                    return session.MasterVolume;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ExecuteAction(object sender, StateUpdateEventArgs args)
        {
            Debug.Assert(Enabled, "Enabled");

            var targets = args.Sessions
                .Where(MatchSessionName)
                .Select(GetValueFromSession)
                .ToList();

            if (targets.Count == 0) return;

            switch (ValueSelectionMethod)
            {
                case ValueSelectionMethod.Average:
                    args.VariableStore[VariableName] = targets.Average();
                    break;

                case ValueSelectionMethod.Median:
                    targets.Sort();
                    args.VariableStore[VariableName] = GetMedian(targets);
                    break;

                case ValueSelectionMethod.Lowest:
                    targets.Sort();
                    args.VariableStore[VariableName] = targets[0];
                    break;

                case ValueSelectionMethod.Highest:
                    targets.Sort();
                    args.VariableStore[VariableName] = targets[targets.Count - 1];
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string GetDetails()
        {
            return $"Set variable \"{VariableName}\" to {ValueSelectionMethod} of {TargetValue} from: {base.GetDetails()}";
        }

        private static float GetMedian(IReadOnlyList<float> sortedPNumbers)
        {
            var size = sortedPNumbers.Count;
            var mid = size / 2;
            var median = size % 2 != 0
                ? sortedPNumbers[mid] 
                : (sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
            return median;
        }
    }
}