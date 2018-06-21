using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class SupercruiseEntry : LogEvent
    {
        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }
    }
}
