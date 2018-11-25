namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Module
    {
        [JsonProperty("Slot")]
        public string Slot { get; set; }

        [JsonProperty("Item")]
        public string Item { get; set; }

        [JsonProperty("On")]
        public bool On { get; set; }

        [JsonProperty("Priority")]
        public long Priority { get; set; }

        [JsonProperty("Health")]
        public double Health { get; set; }

        [JsonProperty("Value")]
        public long? Value { get; set; }

        [JsonProperty("Engineering")]
        public ModuleEngineering Engineering { get; set; }

        [JsonProperty("AmmoInClip")]
        public long? AmmoInClip { get; set; }

        [JsonProperty("AmmoInHopper")]
        public long? AmmoInHopper { get; set; }
    }
}
