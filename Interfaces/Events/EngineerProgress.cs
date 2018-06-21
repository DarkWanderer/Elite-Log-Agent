using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public class EngineerProgress : LogEvent
    {
        [JsonProperty("Engineer")]
        public string Engineer { get; set; }

        [JsonProperty("Progress")]
        public string Progress { get; set; }

        [JsonProperty("Rank")]
        public long Rank { get; set; }
    }
}
