namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

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
