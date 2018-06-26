using DW.ELA.LogModel;
using DW.ELA.LogModel.Events;
using DW.ELA.Plugin.Inara.Model;
using Interfaces;
using MoreLinq;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DW.ELA.Plugin.Inara
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
                    case EngineerProgress e: return Convert(e);

                    // Combat
                    case Interdicted e: return Convert(e);
                    case Interdiction e: return Convert(e);
                    case EscapeInterdiction e: return Convert(e);
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

        private ApiEvent Convert(EscapeInterdiction e)
        {
            return new ApiEvent("addCommanderCombatInterdictionEscape")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdictor },
                    { "isPlayer", e.IsPlayer }
                },
                Timestamp = e.Timestamp
            };
        }

        private ApiEvent Convert(Interdiction e)
        {
            return new ApiEvent("addCommanderCombatInterdiction")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdicted },
                    { "isPlayer", e.IsPlayer }
                },
                Timestamp = e.Timestamp
            };
        }

        private ApiEvent Convert(Interdicted e)
        {
            return new ApiEvent("addCommanderCombatInterdicted")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdictor },
                    { "isPlayer", e.IsPlayer },
                    { "isSubmit", e.Submitted }
                },
                Timestamp = e.Timestamp
            };
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
