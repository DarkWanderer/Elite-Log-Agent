﻿using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public abstract class LocationEventBase : JournalEvent
    {
        [JsonProperty("MarketID")]
        public long? MarketId { get; set; }

        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StationType")]
        public string StationType { get; set; }

        [JsonProperty]
        public string StarSystem { get; set; }

        [JsonProperty]
        public Faction StationFaction { get; set; }

        [JsonProperty]
        public Faction SystemFaction { get; set; }

        [JsonProperty("SystemAddress")]
        public ulong? SystemAddress { get; set; }

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

        [JsonProperty("StationGovernment")]
        public string StationGovernment { get; set; }

        [JsonProperty("StationGovernment_Localised")]
        public string StationGovernmentLocalised { get; set; }

        [JsonProperty("StationAllegiance")]
        public string StationAllegiance { get; set; }

        [JsonProperty("StationServices")]
        public string[] StationServices { get; set; }

        [JsonProperty("StationEconomy")]
        public string StationEconomy { get; set; }

        [JsonProperty("StationEconomy_Localised")]
        public string StationEconomyLocalised { get; set; }

        [JsonProperty("StationEconomies")]
        public StationEconomy[] StationEconomies { get; set; }

        [JsonProperty("Population")]
        public long? Population { get; set; }

        [JsonProperty("Body")]
        public string Body { get; set; }

        [JsonProperty("BodyID")]
        public long? BodyId { get; set; }

        [JsonProperty("BodyType")]
        public string BodyType { get; set; }

        [JsonProperty("Powers")]
        public string[] Powers { get; set; }

        [JsonProperty("PowerplayState")]
        public string PowerplayState { get; set; }

        [JsonProperty("Factions")]
        public Faction[] Factions { get; set; }

        [JsonProperty]
        public ConflictInfo[] Conflicts { get; set; }
    }
}
