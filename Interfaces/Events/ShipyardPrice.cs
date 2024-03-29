﻿using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class ShipyardPrice
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("ShipType")]
        public string ShipType { get; set; }

        [JsonProperty("ShipPrice")]
        public long ShipPrice { get; set; }

        [JsonProperty("ShipType_Localised", NullValueHandling = NullValueHandling.Ignore)]
        public string ShipTypeLocalised { get; set; }
    }
}
