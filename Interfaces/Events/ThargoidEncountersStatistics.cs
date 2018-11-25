namespace DW.ELA.Interfaces.Events
{
    using System.Collections.Generic;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class ThargoidEncountersStatistics
    {
        [JsonProperty("TG_ENCOUNTER_IMPRINT")]
        public long? TgEncounterImprint { get; set; }

        [JsonProperty("TG_ENCOUNTER_TOTAL")]
        public long TgEncounterTotal { get; set; }

        [JsonProperty("TG_ENCOUNTER_TOTAL_LAST_SYSTEM")]
        public string TgEncounterTotalLastSystem { get; set; }

        [JsonProperty("TG_ENCOUNTER_TOTAL_LAST_TIMESTAMP")]
        public string TgEncounterTotalLastTimestamp { get; set; }

        [JsonProperty("TG_ENCOUNTER_TOTAL_LAST_SHIP")]
        public string TgEncounterTotalLastShip { get; set; }

        [JsonProperty("TG_SCOUT_COUNT")]
        public long TgScoutCount { get; set; }
    }
}
