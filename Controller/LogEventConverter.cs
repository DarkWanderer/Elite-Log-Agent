namespace DW.ELA.LogModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using Newtonsoft.Json.Linq;

    public static class LogEventConverter
    {
        private static readonly IReadOnlyDictionary<string, Type> eventTypes;

        static LogEventConverter()
        {
            var baseType = typeof(LogEvent);
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

        public static LogEvent Convert(JObject jObject)
        {
            var eventName = jObject["event"]?.ToString().ToLowerInvariant();
            LogEvent result = null;
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentException("Empty event name", nameof(jObject));

            if (eventTypes.ContainsKey(eventName))
                result = (LogEvent)jObject.ToObject(eventTypes[eventName]);
            else
                result = jObject.ToObject<LogEvent>();

            result.Raw = jObject;
            return result;
        }
    }
}
