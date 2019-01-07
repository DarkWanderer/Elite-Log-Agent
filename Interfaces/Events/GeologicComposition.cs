namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class GeologicComposition
    {
        [JsonProperty("Ice")]
        public double Ice { get; set; }

        [JsonProperty("Rock")]
        public double Rock { get; set; }

        [JsonProperty("Metal")]
        public double Metal { get; set; }
    }
}
