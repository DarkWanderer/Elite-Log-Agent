using System;
using System.Collections.Generic;
using System.Linq;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Events;
using DW.ELA.Plugin.EDDN.Model;
using Newtonsoft.Json.Linq;
using NLog;
using Utility;

namespace DW.ELA.Plugin.EDDN
{
    public class EddnEventConverter : IEventConverter<EddnEvent>
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public string UploaderID = "Unknown";
        public TimeSpan MaxAge = TimeSpan.FromHours(6);

        private IDictionary<string, string> CreateHeader()
        {
            return new Dictionary<string, string>
            {
                ["uploaderID"] = UploaderID,
                ["softwareName"] = AppInfo.Name,
                ["softwareVersion"] = AppInfo.Version
            };
        }

        public IEnumerable<EddnEvent> Convert(LogEvent @event)
        {
            if (@event.Timestamp.Add(MaxAge) < DateTime.UtcNow)
                return Enumerable.Empty<EddnEvent>();
            try
            {
                switch (@event)
                {
                    // Travel events TODO: fix other 2 types
                    //case Docked d:
                    case FsdJump f:
                    //case Scan s:
                    case Location l:
                        return MakeJournalEvent(@event);

                    // Market events TODO: make a pull request to EDDN for case validation
                    case Market e: return ConvertMarketEvent(e);
                    case Outfitting e: return ConvertOutfittingEvent(e);
                    case Shipyard e: return ConvertShipyardEvent(e);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error converting message");
            }
            return Enumerable.Empty<EddnEvent>();
        }

        private IEnumerable<EddnEvent> ConvertShipyardEvent(Shipyard e)
        {
            var @event = new ShipyardEvent
            {
                Header = CreateHeader(),
                Message = new ShipyardMessage
                {
                    StationName = e.StationName,
                    SystemName = e.StarSystem,
                    MarketId = e.MarketId,
                    Timestamp = e.Timestamp,
                    Ships = e.Prices.Select(p => p.ShipType).ToArray()
                }
            };
            yield return @event;
        }

        private IEnumerable<EddnEvent> ConvertOutfittingEvent(Outfitting e)
        {
            if (e.Items == null)
                yield break;

            var items = e.Items
                .Select(i => i.Name)
                .Select(i => i.Replace("hpt_", "Hpt_").Replace("int_", "Int_").Replace("armour_", "Armour_"))
                .ToArray();

            var @event = new OutfittingEvent()
            {
                Header = CreateHeader(),
                Message = new OutfittingMessage()
                {
                    MarketId = e.MarketId,
                    Timestamp = e.Timestamp,
                    StationName = e.StationName,
                    SystemName = e.StarSystem,
                    Modules = items
                }
            };
            yield return @event;
        }

        private IEnumerable<EddnEvent> ConvertMarketEvent(Market e)
        {
            if (e.Items == null)
                yield break;

            var commodities = e.Items
                .Where(i => i.Category != "NonMarketable")
                .Where(i => string.IsNullOrEmpty(i.Legality))
                .ToArray();

            var @event = new CommodityEvent()
            {
                Header = CreateHeader(),
                Message = new CommodityMessage()
                {
                    Timestamp = e.Timestamp,
                    MarketId = e.MarketId,
                    StationName = e.StationName,
                    SystemName = e.StarSystem,
                    Commodities = commodities.Select(ConvertCommodity).ToArray()
                }
            };
            yield return @event;
        }

        private Commodity ConvertCommodity(MarketItem arg)
        {
            return new Commodity()
            {
                BuyPrice = arg.BuyPrice,
                Demand = arg.Demand,
                DemandBracket = arg.DemandBracket,
                MeanPrice = arg.MeanPrice,
                Name = arg.Name.Replace("$", "").Replace("_name;", ""),
                SellPrice = arg.SellPrice,
                Stock = arg.Stock,
                StockBracket = arg.StockBracket
            };
        }

        private IEnumerable<EddnEvent> MakeJournalEvent(LogEvent e) { yield return new JournalEvent { Header = CreateHeader(), Message = Strip(e.Raw) }; }

        private JObject Strip(JObject raw)
        {
            raw = (JObject)raw.DeepClone();
            var attributesToRemove = new List<string>() {
                "CockpitBreach",
                "BoostUsed",
                "FuelLevel",
                "FuelUsed",
                "JumpDist",
                "Latitude",
                "Longitude"
            };

            foreach (var attribute in raw)
                if (attribute.Key.EndsWith("_Localised"))
                    attributesToRemove.Add(attribute.Key);

            foreach (var key in attributesToRemove)
                raw.Remove(key);

            return raw;
        }
    }
}