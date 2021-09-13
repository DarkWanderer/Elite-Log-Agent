using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Location : LocationEventBase
    {
        [JsonProperty("Docked")]
        public bool Docked { get; set; }

        [JsonProperty("Latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("Longitude")]
        public double? Longitude { get; set; }
    }
}
