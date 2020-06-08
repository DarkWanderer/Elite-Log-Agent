namespace DW.ELA.Plugin.EDDN
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Plugin.EDDN.Model;
    using DW.ELA.Utility;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json.Linq;
    using NLog;
    using NLog.Fluent;

    public class EddnEventConverter
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        public TimeSpan MaxAge = TimeSpan.FromMinutes(10); // TODO: should extract to separate class

        private readonly IPlayerStateHistoryRecorder stateHistoryRecorder;

        public EddnEventConverter(IPlayerStateHistoryRecorder stateHistoryRecorder)
        {
            this.stateHistoryRecorder = stateHistoryRecorder;
        }

        public IEnumerable<EddnEvent> Convert(JournalEvent @event, string commanderName)
        {
            if (@event.Timestamp.Add(MaxAge) < DateTime.UtcNow)
                return Enumerable.Empty<EddnEvent>();
            try
            {
                switch (@event)
                {
                    case CarrierJump _:
                    case Docked _:
                    case FsdJump _:
                    case Scan _:
                    case Location _:
                    case SaaSignalsFound _:
                        return MakeJournalEvent(@event, commanderName);

                    case Market e:
                        return ConvertMarketEvent(e, commanderName);
                    case Outfitting e:
                        return ConvertOutfittingEvent(e, commanderName);
                    case Shipyard e:
                        return ConvertShipyardEvent(e, commanderName);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error converting message");
            }
            return Enumerable.Empty<EddnEvent>();
        }

        private IDictionary<string, string> CreateHeader(string commanderName)
        {
            return new Dictionary<string, string>
            {
                ["uploaderID"] = commanderName,
                ["softwareName"] = AppInfo.Name,
                ["softwareVersion"] = AppInfo.Version
            };
        }

        private IEnumerable<EddnEvent> ConvertShipyardEvent(Shipyard e, string commanderName)
        {
            if (e.Prices == null || e.Prices.Length == 0)
                yield break;

            var @event = new ShipyardEvent
            {
                Header = CreateHeader(commanderName),
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

        private IEnumerable<EddnEvent> ConvertOutfittingEvent(Outfitting e, string commanderName)
        {
            if (e.Items == null || e.Items.Length == 0)
                yield break;

            string[] items = e.Items
                .Select(i => i.Name)
                .Select(i => i.Replace("hpt_", "Hpt_").Replace("int_", "Int_").Replace("armour_", "Armour_"))
                .ToArray();

            var @event = new OutfittingEvent()
            {
                Header = CreateHeader(commanderName),
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

        private IEnumerable<EddnEvent> ConvertMarketEvent(Market e, string commanderName)
        {
            if (e.Items == null)
                yield break;

            var commodities = e.Items
                .Where(i => i.Category != "NonMarketable")
                .Where(i => string.IsNullOrEmpty(i.Legality))
                .ToArray();

            var @event = new CommodityEvent()
            {
                Header = CreateHeader(commanderName),
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
                Name = arg.Name.Replace("$", string.Empty).Replace("_name;", string.Empty),
                SellPrice = arg.SellPrice,
                Stock = arg.Stock,
                StockBracket = arg.StockBracket
            };
        }

        private IEnumerable<EddnEvent> MakeJournalEvent(JournalEvent e, string commanderName)
        {
            var @event = new EddnJournalEvent { Header = CreateHeader(commanderName), Message = Strip(e.Raw) };

            if (@event.Message["StarSystem"] == null)
            {
                string system = stateHistoryRecorder.GetPlayerSystem(e.Timestamp);

                // if we can't determine player's location, abort
                if (system != null)
                {
                    @event.Message.Add("StarSystem", system);
                }
                else
                {
                    Log.Error("Unable to determine player location");
                    yield break;
                }
            }

            string starSystem = @event.Message["StarSystem"].ToObject<string>();

            if (@event.Message["StarPos"] == null)
            {
                double[] starPos = stateHistoryRecorder.GetStarPos(starSystem);
                if (starPos == null)
                    yield break; // we don't know what the system coordinates are
                @event.Message.Add("StarPos", new JArray(starPos));
            }

            if (@event.Message["SystemAddress"] == null)
            {
                ulong? systemAddress = stateHistoryRecorder.GetSystemAddress(starSystem);
                if (systemAddress != null)
                    @event.Message.Add("SystemAddress", systemAddress);
            }

            if (Log.IsTraceEnabled)
            {
                Log.Trace()
                    .Message("Converted message")
                    .Property("source", Serialize.ToJson(e))
                    .Property("output", Serialize.ToJson(@event))
                    .Write();
            }

            yield return @event;
        }

        private static JObject Strip(JObject raw)
        {
            raw = (JObject)raw.DeepClone();

            var attributesToRemove = new List<string>()
            {
                "ActiveFine",
                "BoostUsed",
                "CockpitBreach",
                "FuelLevel",
                "FuelUsed",
                "JumpDist",
                "Latitude",
                "Longitude",
                "Wanted"
            };

            foreach (string key in attributesToRemove)
                raw.Remove(key);

            if (raw["Factions"] is JArray factions)
            {
                foreach (JObject faction in factions)
                {
                    faction.Remove("HappiestSystem");
                    faction.Remove("HomeSystem");
                    faction.Remove("MyReputation");
                    faction.Remove("SquadronFaction");
                }
            }

            WalkNode(raw, RemoveLocalizedProperties, null);

            return raw;
        }

        private static void RemoveLocalizedProperties(JObject obj)
        {
            var keysToRemove = new List<string>();

            foreach (var property in obj)
            {
                if (property.Key.EndsWith("_Localised"))
                    keysToRemove.Add(property.Key);
            }

            foreach (string key in keysToRemove)
                obj.Remove(key);
        }

        private static void WalkNode(
                                JToken node,
                                Action<JObject> objectAction,
                                Action<JProperty> propertyAction)
        {
            if (node.Type == JTokenType.Object)
            {
                objectAction?.Invoke((JObject)node);

                foreach (var child in node.Children<JProperty>())
                {
                    propertyAction?.Invoke(child);
                    WalkNode(child.Value, objectAction, propertyAction);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (var child in node.Children())
                {
                    WalkNode(child, objectAction, propertyAction);
                }
            }
        }
    }
}
