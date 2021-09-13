using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class LoadGame : JournalEvent
    {
        [JsonProperty("Commander")]
        public string Commander { get; set; }

        [JsonProperty("Horizons")]
        public bool? Horizons { get; set; }

        [JsonProperty("Ship")]
        public string Ship { get; set; }

        [JsonProperty("Ship_Localised")]
        public string ShipLocalised { get; set; }

        [JsonProperty("ShipID")]
        public long ShipId { get; set; }

        [JsonProperty]
        public string ShipName { get; set; }

        [JsonProperty]
        public string ShipIdent { get; set; }

        [JsonProperty]
        public double? FuelLevel { get; set; }

        [JsonProperty]
        public double? FuelCapacity { get; set; }

        [JsonProperty("GameMode")]
        public string GameMode { get; set; }

        [JsonProperty("Credits")]
        public long Credits { get; set; }

        [JsonProperty("Loan")]
        public long Loan { get; set; }

        [JsonProperty]
        public string Group { get; set; }

        [JsonProperty("FID")]
        public string FrontierId { get; set; }

        [JsonProperty]
        public bool? StartLanded { get; set; }
    }
}
