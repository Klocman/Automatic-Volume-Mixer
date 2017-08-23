using System.ComponentModel;

namespace Avm.Storage.Triggers
{
    public abstract class NameFilterTriggerBase : NameFilterBase
    {
        [Category("Basic")]
        [Description("Invert the condition's result. If it fires, don't fire. If it doesn't fire, fire.")]
        [DefaultValue(false)]
        public bool InvertResult { get; set; }
    }

    public abstract class BasicInfoBaseTriggerBase : BasicInfoBase
    {
        [Category("Basic")]
        [Description("Invert the condition's result. If it fires, don't fire. If it doesn't fire, fire.")]
        [DefaultValue(false)]
        public bool InvertResult { get; set; }
    }
}