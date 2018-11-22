namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Commander : LogEvent
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("FID")]
        public string FrontierId { get; set; }
    }
}
