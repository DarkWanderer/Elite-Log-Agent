namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class Composition
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Percent")]
        public double Percent { get; set; }
    }
}
