namespace DW.ELA.Controller
{
    using System.Collections.Generic;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json.Linq;

    public class EventRawJsonExtractor : IEventConverter<JObject>
    {
        public IEnumerable<JObject> Convert(LogEvent @event)
        {
            yield return @event.Raw;
        }
    }
}
