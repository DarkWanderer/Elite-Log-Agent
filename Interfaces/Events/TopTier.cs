namespace DW.ELA.Interfaces.Events
{
    using System;
    using Newtonsoft.Json;

    public class TopTier
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Bonus")]
        public string Bonus { get; set; }
    }
}
