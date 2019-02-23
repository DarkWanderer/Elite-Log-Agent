﻿namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Powerplay : LogEvent
    {
        [JsonProperty]
        public string Power { get; set; }

        [JsonProperty]
        public int Rank { get; set; }

        [JsonProperty]
        public int? Merits { get; set; }

        [JsonProperty]
        public int? Votes { get; set; }

        [JsonProperty]
        public int? TimePledged { get; set; }
    }
}
