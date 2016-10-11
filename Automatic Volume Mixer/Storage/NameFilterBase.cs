using System;
using System.ComponentModel;
using System.Linq;
using Avm.Daemon;
using Klocman.Extensions;

namespace Avm.Storage
{
    [DefaultProperty(nameof(FilterText))]
    public abstract class NameFilterBase : BasicInfoBase
    {
        [Category("Application filter")]
        [Description("Text to be compared against the application names. If the comparison " +
                     "fails the application will not be included in any further processing inside of this trigger.")]
        [DefaultValue("")]
        public string FilterText { get; set; } = "";

        [Category("Application filter")]
        [Description("Type of the comparison. No filtering will pass all applications through.")]
        [DefaultValue(FilteringTypes.NoFiltering)]
        public FilteringTypes FilteringType { get; set; } = FilteringTypes.NoFiltering;

        [Category("Application filter")]
        [Description("Always filter out the \"System Sounds\" session.")]
        [DefaultValue(true)]
        public bool ExcludeSystemSounds { get; set; } = true;

        public override string GetDetails()
        {
            return $@"Filter using {FilteringType} by {FilterText}";
        }

        protected bool MatchUpdate(StateUpdateEventArgs args)
        {
            return args.Sessions.Any(MatchSessionName);
        }

        protected bool MatchSessionName(AudioSession target)
        {
            if (ExcludeSystemSounds && target.IsSystemSoundSession)
                return false;

            return MatchName(target.DisplayName);
        }

        protected bool MatchName(string target)
        {
            switch (FilteringType)
            {
                case FilteringTypes.Containing:
                    return target.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase);
                case FilteringTypes.NotContaining:
                    return !target.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase);
                case FilteringTypes.NoFiltering:
                    return true;

                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}