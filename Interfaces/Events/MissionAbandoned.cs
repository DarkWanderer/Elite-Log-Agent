using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class MissionAbandoned : LogEvent
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("MissionID")]
        public long MissionId { get; set; }
    }
}
