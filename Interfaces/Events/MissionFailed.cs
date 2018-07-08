using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class MissionFailed : LogEvent
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("MissionID")]
        public long MissionId { get; set; }
    }
}
