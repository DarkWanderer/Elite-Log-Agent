namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Loadout : LogEvent
    {
        [JsonProperty("Ship")]
        public string Ship { get; set; }

        [JsonProperty("ShipID")]
        public long ShipId { get; set; }

        [JsonProperty("ShipName")]
        public string ShipName { get; set; }

        [JsonProperty("ShipIdent")]
        public string ShipIdent { get; set; }

        [JsonProperty("HullValue")]
        public long? HullValue { get; set; }

        [JsonProperty("ModulesValue")]
        public long? ModulesValue { get; set; }

        [JsonProperty("Rebuy")]
        public long? Rebuy { get; set; }

        [JsonProperty]
        public bool? Hot { get; set; }

        [JsonProperty]
        public double? HullHealth { get; set; }

        [JsonProperty("Modules")]
        public Module[] Modules { get; set; }
    }
}
