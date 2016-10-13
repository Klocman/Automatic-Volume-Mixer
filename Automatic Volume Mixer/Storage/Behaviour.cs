using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Klocman.Extensions;

namespace Avm.Storage
{
    public class Behaviour : IBasicInfo
    {
        public Behaviour()
        {
            Id = Guid.NewGuid();
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
            Id = other.Id;
            Enabled = other.Enabled;
            TriggeringKind = other.TriggeringKind;
            Group = other.Group;
            MinimalTimedTriggerDelay = other.MinimalTimedTriggerDelay;
            CooldownPeriod = other.CooldownPeriod;

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
        
        /// <summary>
        /// After the behaviour triggers, disable it for this many seconds.
        /// </summary>
        public double CooldownPeriod { get; set; }

        [DefaultValue("")]
        public string Group { get; set; }

        public string GetDetails()
        {
            var sb = new StringBuilder();
            sb.Append(Triggers.Count);
            sb.Append(" trigger");
            sb.AppendIf(Triggers.Count != 1, "s");
            sb.Append(", ");
            sb.Append(Conditions.Count);
            sb.Append(" condition");
            sb.AppendIf(Conditions.Count != 1, "s");
            sb.Append(", and ");
            sb.Append(Actions.Count);
            sb.Append(" action");
            sb.AppendIf(Actions.Count != 1, "s");
            sb.Append(".");
            return sb.ToString();
        }
        
        public string Name { get; set; }
        public bool Enabled { get; set; }
        [Browsable(false)]
        public Guid Id { get; set; }

        public object Clone()
        {
            return new Behaviour(this);
        }
    }
}