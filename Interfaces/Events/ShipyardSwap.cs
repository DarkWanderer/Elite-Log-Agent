namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class ShipyardSwap : JournalEvent
    {
        [JsonProperty("ShipType")]
        public string ShipType { get; set; }

        [JsonProperty("ShipType_Localised")]
        public string ShipTypeLocalised { get; set; }

        [JsonProperty("ShipID")]
        public long ShipId { get; set; }

        [JsonProperty("StoreOldShip")]
        public string StoreOldShip { get; set; }

        [JsonProperty("StoreShipID")]
        public long StoreShipId { get; set; }

        [JsonProperty("MarketID")]
        public long? MarketId { get; set; }
    }
}
