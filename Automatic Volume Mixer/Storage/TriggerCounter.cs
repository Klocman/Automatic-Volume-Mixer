using System;
using System.Collections.Generic;

namespace Avm.Storage
{
    public sealed class TriggerCounter
    {
        private static Dictionary<Guid, TriggerCounter> Counters { get; } = new Dictionary<Guid, TriggerCounter>();

        public static void BumpCounter(IBasicInfo item, DateTime newDate)
        {
            var trc = Counters.ContainsKey(item.Id) ? Counters[item.Id] : (Counters[item.Id] = new TriggerCounter());
            trc.TriggerCount++;
            trc.LastTriggerTime = newDate;
        }

        public static TriggerCounter GetCounter(IBasicInfo item)
        {
            return Counters.ContainsKey(item.Id) ? Counters[item.Id] : new TriggerCounter();
        }

        public override string ToString()
        {
            return TriggerCount <= 0 ? "Never succedeed" : $"Succeeded {TriggerCount} times, last on {LastTriggerTime}";
        }

        public DateTime LastTriggerTime { get; set; }
        public long TriggerCount { get; set; }
    }
}