namespace DW.ELA.LogModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using Newtonsoft.Json.Linq;

    public static class JournalEventConverter
    {
        private static readonly IReadOnlyDictionary<string, Type> eventTypes;

        static JournalEventConverter()
        {
            var baseType = typeof(JournalEvent);
            var exampleType = typeof(LoadGame);

            eventTypes = baseType
               .Assembly
               .GetTypes()
               .Where(t => t.Namespace == exampleType.Namespace)
               .Where(t => t.BaseType == baseType)
               .ToDictionary(t => t.Name.ToLowerInvariant(), t => t);
            Debug.Assert(eventTypes.Count > 0, "Must have events");
            Debug.Assert(eventTypes.Values.Contains(exampleType), "Event LoadGame not loaded");
        }

        public static JournalEvent Convert(JObject jObject)
        {
            string eventName = jObject["event"]?.ToString()?.ToLowerInvariant();
            JournalEvent result;
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentException("Empty event name", nameof(jObject));

            if (eventTypes.ContainsKey(eventName))
                result = (JournalEvent)jObject.ToObject(eventTypes[eventName]);
            else
                result = jObject.ToObject<JournalEvent>();

            result.Raw = jObject;
            return result;
        }
    }
}
