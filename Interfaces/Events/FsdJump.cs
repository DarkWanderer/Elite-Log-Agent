using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace DW.ELA.LogModel.Events
{
    public class FsdJump : LogEvent
    {
        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("SystemAddress")]
        public long SystemAddress { get; set; }

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
    }

    public class Faction
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("FactionState")]
        public FactionState FactionState { get; set; }

        [JsonProperty("Government")]
        public string Government { get; set; }

        [JsonProperty("Influence")]
        public double Influence { get; set; }

        [JsonProperty("Allegiance")]
        public string Allegiance { get; set; }

        [JsonProperty("PendingStates")]
        public FactionStateTrend[] PendingStates { get; set; }

        [JsonProperty("RecoveringStates")]
        public FactionStateTrend[] RecoveringStates { get; set; }
    }

    public partial class FactionStateTrend
    {
        [JsonProperty("State")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FactionState State { get; set; }

        [JsonProperty("Trend")]
        public long Trend { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FactionState
    {
        None,
        [EnumMember(Value = "Civil Unrest")] CivilUnrest,
        [EnumMember(Value = "Civil War")] CivilWar,
        Boom,
        Bust,
        Election,
        Expansion,
        Famine,
        Investment,
        Lockdown,
        Outbreak,
        Retreat,
        War
    };
}
