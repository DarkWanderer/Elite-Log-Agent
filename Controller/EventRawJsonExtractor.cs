using DW.ELA.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DW.ELA.Controller
{
    public class EventRawJsonExtractor : IEventConverter<JObject>
    {
        public IEnumerable<JObject> Convert(LogEvent @event)
        {
            yield return @event.Raw;
        }
    }
}
