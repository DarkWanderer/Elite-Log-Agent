using DW.ELA.LogModel;
using DW.ELA.LogModel.Events;
using InaraUpdater.Model;
using Interfaces;
using MoreLinq;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace InaraUpdater
{
    public class InaraEventConverter
    {
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public InaraEventConverter(IPlayerStateHistoryRecorder playerStateRecorder)
        {
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
        }

        public ApiEvent Convert(LogEvent @event)
        {
            try
            {
                var eventName = @event.Event;
                switch (@event)
                {
                    // Generic
                    case LoadGame e: return Convert(e);
                    case Materials e: return Convert(e);
                    case Statistics e: return Convert(e);

                    // Travel
                    case FsdJump e: return Convert(e);
                    case Docked e: return Convert(e);

                    // Engineers
                    case EngineerProgress e: return Convert(e) ;

                    // Combat
                    //case "Interdicted":
                    //case "Interdiction":
                    //case "EscapeInterdiction":
                    //    Queue(ToInterdictionEvent(@event)); break;
                    case PvpKill e: return Convert(e);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error in OnNext");
            }
            return null;
        }

        private ApiEvent Convert(PvpKill @event)
        {
            var timestamp = @event.Timestamp;
            return new ApiEvent("addCommanderCombatKill")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(timestamp) },
                    { "opponentName", @event.Victim },
                },
                Timestamp = timestamp
            };
        }

        private ApiEvent ToInterdictionEvent(JObject @event)
        {
            string eventType;
            switch (@event["event"].ToString())
            {
                case "Interdicted": eventType = "addCommanderCombatInterdicted"; break;
                case "Interdiction": eventType = "addCommanderCombatInterdiction"; break;
                case "EscapeInterdiction": eventType = "addCommanderCombatInterdictionEscape"; break;
                default: throw new ArgumentOutOfRangeException(nameof(@eventType));
            }

            if (@event["StarSystem"] == null)
                return null;

            return new ApiEvent(eventType)
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event["StarSystem"]?.ToString() },
                    { "opponentName", (@event["Interdicted"] ?? @event["Interdictor"] ?? "Unknown").ToString() },
                    { "isPlayer", @event["IsPlayer"]?.ToObject<long>() },
                    { "isSuccess", @event["Success"]?.ToObject<bool>() }
                },
                Timestamp = @event["Timestamp"].ToObject<DateTime>()
            };
        }

        private ApiEvent Convert(Statistics @event)
        {
            return new ApiEvent("setCommanderGameStatistics")
            {
                EventData = @event.Raw,
                Timestamp = @event.Timestamp
            };
        }

        private ApiEvent Convert(Docked @event)
        {
            var timestamp = @event.Timestamp;
            return new ApiEvent("addCommanderTravelDock")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event.StarSystem },
                    { "stationName", @event.StationName},
                    { "marketID", @event.MarketId },
                    { "shipGameID", playerStateRecorder.GetPlayerShipId(timestamp) },
                    { "shipType", playerStateRecorder.GetPlayerShipType(timestamp) }
                },
                Timestamp = timestamp
            };
        }

        private ApiEvent Convert(FsdJump @event)
        {
            var timestamp = @event.Timestamp;
            return new ApiEvent("addCommanderTravelFSDJump")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event.StarSystem },
                    { "jumpDistance", @event.JumpDist },
                    { "shipGameID", playerStateRecorder.GetPlayerShipId(timestamp) },
                    { "shipType", playerStateRecorder.GetPlayerShipType(timestamp) }
                },
                Timestamp = timestamp
            };
        }

        private ApiEvent Convert(Materials @event)
        {
            var materialCounts = @event.RawMats.Concat(@event.Manufactured).Concat(@event.Encoded)
                .ToDictionary(mat => mat.Name, mat => mat.Count);

            return new ApiEvent("setCommanderInventoryMaterials")
            {
                EventData = materialCounts
                    .Select(kvp => new { itemName = kvp.Key, itemCount = kvp.Value })
                    .OrderBy(x => x.itemName)
                    .ToArray(),
                Timestamp = @event.Timestamp
            };
        }

        private ApiEvent Convert(LoadGame @event)
        {
            return new ApiEvent("setCommanderCredits")
            {
                EventData = new Dictionary<string, object> {
                    { "commanderCredits", @event.Credits },
                    { "commanderLoan", @event.Loan }
                },
                Timestamp = @event.Timestamp
            };
        }

        private ApiEvent Convert(EngineerProgress @event)
        {
            return new ApiEvent("setCommanderRankEngineer")
            {
                EventData = new Dictionary<string, object> {
                    { "engineerName", @event.Engineer },
                    { "rankStage", @event.Progress },
                    { "rankValue", @event.Rank }
                },
                Timestamp = @event.Timestamp
            };
        }
    }
}
