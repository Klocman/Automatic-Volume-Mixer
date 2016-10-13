using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Avm.Daemon;
using Klocman.Extensions;

namespace Avm.Storage
{
    public class BehaviourManager
    {
        private readonly List<BehaviourInfo> _behaviours;
        private readonly Dictionary<string, BehaviourGroup> _groupedTasks;
        private readonly XmlSerializer _xmls = new XmlSerializer(typeof(Behaviour));

        public BehaviourManager()
        {
            _behaviours = new List<BehaviourInfo>();
            _groupedTasks = new Dictionary<string, BehaviourGroup>();
        }

        public bool Enabled { get; set; } = true;
        public event EventHandler BehavioursChanged;

        public IEnumerable<Behaviour> GetBehaviours() => _behaviours.Select(x => x.Behaviour);

        private static XElement SerializeList(XName rootName, IEnumerable items, bool indent)
        {
            var root = new XElement(rootName);

            foreach (var item in items)
            {
                var entry = new XElement("Entry");
                root.Add(entry);

                var trigType = item.GetType();
                entry.Add(new XAttribute("type", trigType.FullName));

                var xmls = new XmlSerializer(trigType);
                entry.Value = xmls.Serialize(item, indent);
            }

            return root;
        }

        public static IEnumerable DeserializeList(XElement root)
        {
            if (root == null || !root.HasElements)
                return Enumerable.Empty<object>();

            return from xElement in root.Elements()
                   let strType = xElement.Attribute("type")?.Value
                   where !string.IsNullOrEmpty(strType)
                   let type = Type.GetType(strType)
                   where type != null
                   let xmls = new XmlSerializer(type)
                   select xmls.Deserialize(xElement.Value);
        }

        public string SerializeBehaviours(bool disableFormatting)
        {
            var xdoc = new XDocument();
            var root = new XElement("BehaviourList");
            xdoc.Add(root);
            foreach (var behaviour in _behaviours.Select(x => x.Behaviour))
            {
                var xBase = new XElement("Behaviour");
                root.Add(xBase);

                var xProp = new XElement("Properties");
                xBase.Add(xProp);

                xProp.Value = _xmls.Serialize(behaviour, !disableFormatting);

                xBase.Add(SerializeList(nameof(behaviour.Triggers), behaviour.Triggers, !disableFormatting));
                xBase.Add(SerializeList(nameof(behaviour.Conditions), behaviour.Conditions, !disableFormatting));
                xBase.Add(SerializeList(nameof(behaviour.Actions), behaviour.Actions, !disableFormatting));
            }
            return xdoc.ToString(disableFormatting ? SaveOptions.DisableFormatting : SaveOptions.None);
        }

        public void DeserializeBehaviours(string serializedBehaviours, bool clearPrevious)
        {
            var xdoc = XDocument.Parse(serializedBehaviours);
            var root = xdoc.Root;
            if (root == null || !root.HasElements)
                throw new ArgumentException("Invalid serialized data", nameof(serializedBehaviours));

            var results = new List<Behaviour>();
            foreach (var xBase in root.Elements())
            {
                var xProp = xBase.Element("Properties");
                if (xProp == null)
                    continue;

                //var xPropReader = xProp.CreateReader();
                var behaviour = (Behaviour)_xmls.Deserialize(xProp.Value);
                //xPropReader.Dispose();

                behaviour.Triggers = new List<ITrigger>(DeserializeList(xBase.Element(nameof(behaviour.Triggers)))
                    .Cast<ITrigger>());
                behaviour.Conditions = new List<ITrigger>(DeserializeList(xBase.Element(nameof(behaviour.Conditions)))
                    .Cast<ITrigger>());
                behaviour.Actions = new List<IAction>(DeserializeList(xBase.Element(nameof(behaviour.Actions)))
                    .Cast<IAction>());

                results.Add(behaviour);
            }

            if (clearPrevious)
            {
                _behaviours.Clear();
                _groupedTasks.Clear();
            }
            _behaviours.AddRange(results.Select(x => new BehaviourInfo(x)));
            BehavioursChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddBehaviour(Behaviour item)
        {
            _behaviours.Add(new BehaviourInfo(item));
            BehavioursChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveBehaviour(Behaviour item)
        {
            _behaviours.RemoveAll(x => x.Behaviour.Equals(item));
            BehavioursChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public void ProcessEvents(object sender, StateUpdateEventArgs args)
        {
            //TODO: update by id's to the groups, abort tasks that run on removed sessions
            foreach (var behaviourInfo in _behaviours)
            {
                ProcessEvent(behaviourInfo, sender, args);
            }
        }

        private void ProcessEvent(BehaviourInfo behaviourInfo, object sender, StateUpdateEventArgs args)
        {
            if (!Enabled) return;

            var targetBehaviour = behaviourInfo.Behaviour;

            if (!targetBehaviour.Enabled) return;

            //TODO process conditions before or after further checks
            var result = false;

            var triggerTester = new Func<ITrigger, bool>(tr =>
            {
                try
                {
                    if (tr.ProcessTrigger(sender, args))
                    {
                        TriggerCounter.BumpCounter(tr, args.SnapshotTime);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    DebugTools.WriteException(ex);
                }
                return false;
            });

            var triggerTriggered = targetBehaviour.Triggers.Any(triggerTester);

            if (triggerTriggered)
                triggerTriggered = targetBehaviour.Conditions.All(triggerTester);

            switch (targetBehaviour.TriggeringKind)
            {
                case TriggeringMode.RisingEdge:
                    result = !behaviourInfo.LastTriggerState && triggerTriggered;
                    break;

                case TriggeringMode.FallingEdge:
                    result = behaviourInfo.LastTriggerState && !triggerTriggered;
                    break;

                case TriggeringMode.Always:
                    result = triggerTriggered;
                    break;

                case TriggeringMode.BothEdges:
                    result = (!behaviourInfo.LastTriggerState && triggerTriggered)
                             || (behaviourInfo.LastTriggerState && !triggerTriggered);
                    break;

                case TriggeringMode.Timed:
                    if (triggerTriggered)
                    {
                        if (behaviourInfo.InitialTriggerTime.Equals(DateTime.MaxValue))
                            break;

                        if (behaviourInfo.InitialTriggerTime.Equals(DateTime.MinValue))
                            behaviourInfo.InitialTriggerTime = args.SnapshotTime;

                        if (behaviourInfo.InitialTriggerTime.AddSeconds(targetBehaviour.MinimalTimedTriggerDelay)
                            <= args.SnapshotTime)
                        {
                            behaviourInfo.InitialTriggerTime = DateTime.MaxValue;
                            result = true;
                        }
                    }
                    else
                    {
                        behaviourInfo.InitialTriggerTime = DateTime.MinValue;
                    }
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }

            behaviourInfo.LastTriggerState = triggerTriggered;

            if (!result) return;

            Action runActions = () =>
            {
                foreach (var action in targetBehaviour.Actions)
                {
                    try
                    {
                        action.ExecuteAction(sender, args);
                        TriggerCounter.BumpCounter(action, args.SnapshotTime);
                    }
                    catch (Exception ex)
                    {
                        DebugTools.WriteException(ex);
                    }
                }
            };

            if (string.IsNullOrEmpty(targetBehaviour.Group))
            {
                //BUG different than running using BehaviourGroup, can queue next run before it ends
                // Task doesnt exist yet or it has been completed
                if (behaviourInfo.UngroupedTask?.IsCompleted != false)
                {
                    behaviourInfo.UngroupedTask = Task.Run(runActions);
                }
            }
            else
            {
                BehaviourGroup behaviourGroup;

                if (!_groupedTasks.TryGetValue(targetBehaviour.Group, out behaviourGroup))
                {
                    behaviourGroup = new BehaviourGroup();
                    _groupedTasks.Add(targetBehaviour.Group, behaviourGroup);
                }

                behaviourGroup.RunBehaviour(runActions, targetBehaviour);
            }

            TriggerCounter.BumpCounter(behaviourInfo.Behaviour, args.SnapshotTime);
        }

        public sealed class BehaviourInfo
        {
            public BehaviourInfo(Behaviour behaviour)
            {
                Behaviour = behaviour;
                InitialTriggerTime = DateTime.MinValue;
            }

            public Behaviour Behaviour { get; }
            public bool LastTriggerState { get; set; }
            public DateTime InitialTriggerTime { get; set; }
            public Task UngroupedTask { get; set; }
        }

        private sealed class BehaviourGroup
        {
            private readonly List<KeyValuePair<Behaviour, Action>> _queuedActions =
                new List<KeyValuePair<Behaviour, Action>>();

            private Task _queueExecuteTask;

            public void RunBehaviour(Action action, Behaviour tag)
            {
                lock (_queuedActions)
                {
                    _queuedActions.RemoveAll(x => x.Key.Equals(tag));
                    _queuedActions.Add(new KeyValuePair<Behaviour, Action>(tag, action));

                    if (_queueExecuteTask?.IsCompleted != false)
                        _queueExecuteTask = Task.Factory.StartNew(ExecuteQueued);
                }
            }

            private void ExecuteQueued()
            {
                while (true)
                {
                    Action actionToRun;
                    lock (_queuedActions)
                    {
                        if (_queuedActions.Count <= 0) return;

                        actionToRun = _queuedActions[0].Value;
                        _queuedActions.RemoveAt(0);
                    }
                    actionToRun();
                }
            }
        }
    }
    
    public sealed class TriggerCounter
    {
        private static Dictionary<Guid, TriggerCounter> Counters { get; } = new Dictionary<Guid, TriggerCounter>();

        public static void BumpCounter(IBasicInfo item, DateTime newDate)
        {
            var trc = Counters.ContainsKey(item.ID) ? Counters[item.ID] : (Counters[item.ID] = new TriggerCounter());
            trc.TriggerCount++;
            trc.LastTriggerTime = newDate;
        }

        public static TriggerCounter GetCounter(IBasicInfo item)
        {
            return Counters.ContainsKey(item.ID) ? Counters[item.ID] : new TriggerCounter();
        }
        
        public override string ToString()
        {
            return TriggerCount <= 0 ? "Never triggered" : $"Triggered {TriggerCount} times, last on {LastTriggerTime}";
        }

        public DateTime LastTriggerTime { get; set; }
        public long TriggerCount { get; set; }
    }
}