using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Avm.Storage
{
    public class Behaviour : IBasicInfo
    {
        public Behaviour()
        {
            Name = "New event";
            Enabled = true;
            TriggeringKind = TriggeringMode.RisingEdge;
            Group = string.Empty;

            Triggers = new List<ITrigger>();
            Conditions = new List<ITrigger>();
            Actions = new List<IAction>();
        }

        public Behaviour(Behaviour other)
        {
            Name = other.Name;
            Enabled = other.Enabled;
            TriggeringKind = other.TriggeringKind;
            Group = other.Group;
            MinimalTimedTriggerDelay = other.MinimalTimedTriggerDelay;

            Triggers = new List<ITrigger>(other.Triggers.Select(trigger => (ITrigger) trigger.Clone()));
            Conditions = new List<ITrigger>(other.Conditions.Select(condition => (ITrigger) condition.Clone()));
            Actions = new List<IAction>(other.Actions.Select(action => (IAction) action.Clone()));
        }

        [XmlIgnore]
        public IList<ITrigger> Triggers { get; set; }

        [XmlIgnore]
        public IList<ITrigger> Conditions { get; set; }

        [XmlIgnore]
        public IList<IAction> Actions { get; set; }

        public TriggeringMode TriggeringKind { get; set; }

        /// <summary>
        ///     How many seconds to wait for before registering the trigger.
        ///     If during any update in between the trigger fails, the timer is reset.
        /// </summary>
        public double MinimalTimedTriggerDelay { get; set; }

        [DefaultValue("")]
        public string Group { get; set; }

        public string GetDetails()
        {
            return $@"{Triggers.Count} trigger(s), {Conditions.Count} condition(s) and {Actions.Count} action(s).";
        }

        public string Name { get; set; }
        public bool Enabled { get; set; }

        public object Clone()
        {
            return new Behaviour(this);
        }
    }
}