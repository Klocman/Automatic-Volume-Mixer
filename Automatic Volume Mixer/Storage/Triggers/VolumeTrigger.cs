using System.ComponentModel;
using System.Linq;
using Avm.Daemon;

namespace Avm.Storage.Triggers
{
    public enum VolumeType
    {
        Peak,
        Master
        //TODO Global
    }

    [DefaultProperty(nameof(VolumeValue))]
    public class VolumeTrigger : RunningSessionTrigger, ITrigger
    {
        [Category("Volume")]
        [Description("Volume to compare against.")]
        [DefaultValue(0.01f)]
        public float VolumeValue { get; set; } = 0.01f;

        [Category("Volume")]
        [Description("Type of comparison.")]
        [DefaultValue(VolumeComparisonType.AnyAbove)]
        public VolumeComparisonType ComparisonType { get; set; } = VolumeComparisonType.AnyAbove;

        [Category("Volume")]
        [Description("Value to compare volume against. Master stands for relative max application volume.")]
        [DefaultValue(VolumeType.Peak)]
        public VolumeType ComparisonTarget { get; set; } = VolumeType.Peak;

        public override string GetDetails()
        {
            return $@"{ComparisonTarget} volume is {ComparisonType} {VolumeValue}; {base.GetDetails()}";
        }

        public override bool ProcessTrigger(object sender, StateUpdateEventArgs args)
        {
            if (!Enabled) return false;

            foreach (var compareSuccess in args.Sessions.Where(x => MatchSessionName(x.Value))
                .Select(x => ComparePeakValue(ComparisonTarget == VolumeType.Peak
                ? x.Value.PeakValue
                : x.Value.MasterVolume))
                )
            {
                switch (ComparisonType)
                {
                    case VolumeComparisonType.AnyAbove:
                    case VolumeComparisonType.AnyBelow:
                        if (compareSuccess) return true;
                        break;

                    case VolumeComparisonType.AllBelow:
                    case VolumeComparisonType.AllAbove:
                        if (!compareSuccess) return false;
                        break;

                    default:
                        throw new InvalidEnumArgumentException();
                }
            }

            switch (ComparisonType)
            {
                case VolumeComparisonType.AnyAbove:
                case VolumeComparisonType.AnyBelow:
                    return false;

                case VolumeComparisonType.AllBelow:
                case VolumeComparisonType.AllAbove:
                    return true;

                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        private bool ComparePeakValue(float compareTarget)
        {
            switch (ComparisonType)
            {
                case VolumeComparisonType.AnyAbove:
                case VolumeComparisonType.AllAbove:
                    return (VolumeValue < compareTarget);
                case VolumeComparisonType.AllBelow:
                case VolumeComparisonType.AnyBelow:
                    return (VolumeValue > compareTarget);

                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}