using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class Loadout : JournalEvent
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

        [JsonProperty]
        public double? UnladenMass { get; set; }

        [JsonProperty]
        public int? CargoCapacity { get; set; }

        [JsonProperty]
        public double? MaxJumpRange { get; set; }

        [JsonProperty]
        public FuelCapacity? FuelCapacity { get; set; }
    }
}
