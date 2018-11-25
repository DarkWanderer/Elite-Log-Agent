namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class FsdJump : LogEvent
    {
        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("SystemAddress")]
        public long? SystemAddress { get; set; }

        [JsonProperty("StarPos")]
        public double[] StarPos { get; set; }

        [JsonProperty("SystemAllegiance")]
        public string SystemAllegiance { get; set; }

        [JsonProperty("SystemEconomy")]
        public string SystemEconomy { get; set; }

        [JsonProperty("SystemEconomy_Localised")]
        public string SystemEconomyLocalised { get; set; }

        [JsonProperty("SystemSecondEconomy")]
        public string SystemSecondEconomy { get; set; }

        [JsonProperty("SystemSecondEconomy_Localised")]
        public string SystemSecondEconomyLocalised { get; set; }

        [JsonProperty("SystemGovernment")]
        public string SystemGovernment { get; set; }

        [JsonProperty("SystemGovernment_Localised")]
        public string SystemGovernmentLocalised { get; set; }

        [JsonProperty("SystemSecurity")]
        public string SystemSecurity { get; set; }

        [JsonProperty("SystemSecurity_Localised")]
        public string SystemSecurityLocalised { get; set; }

        [JsonProperty("Population")]
        public long Population { get; set; }

        [JsonProperty("Powers")]
        public string[] Powers { get; set; }

        [JsonProperty("PowerplayState")]
        public string PowerplayState { get; set; }

        [JsonProperty("JumpDist")]
        public double JumpDist { get; set; }

        [JsonProperty("FuelUsed")]
        public double FuelUsed { get; set; }

        [JsonProperty("FuelLevel")]
        public double FuelLevel { get; set; }

        [JsonProperty("Factions")]
        public Faction[] Factions { get; set; }

        [JsonProperty("SystemFaction")]
        public string SystemFaction { get; set; }

        [JsonProperty("FactionState")]
        public FactionState? FactionState { get; set; }
    }
}
