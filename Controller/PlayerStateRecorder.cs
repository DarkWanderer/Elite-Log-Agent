using Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NLog;

namespace Controller
{
    public class PlayerStateRecorder : IPlayerStateHistoryRecorder
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly SortedList<DateTime, string> playerLocations = new SortedList<DateTime, string>();
        private readonly SortedList<DateTime, ShipRecord> playerShipReferences = new SortedList<DateTime, ShipRecord>();

        public string GetPlayerSystem(DateTime atTime)
        {
            try
            {
                lock (playerLocations)
                    return playerLocations
                            .Where(l => l.Key < atTime)
                            .DefaultIfEmpty()
                            .MaxBy(l => l.Key).Value;
            }
            catch { return null; }
        }

        public long? GetPlayerShipId(DateTime atTime)
        {
            try
            {
                lock (playerShipReferences)
                    return playerShipReferences
                        .Where(l => l.Key < atTime)
                        .DefaultIfEmpty()
                        .MaxBy(l => l.Key)
                        .Value.ShipID;
            }
            catch { return null; }
        }

        public string GetPlayerShipType(DateTime atTime)
        {
            lock (playerLocations)
                try
                {
                    return playerShipReferences
                        .Where(l => l.Key < atTime)
                        .DefaultIfEmpty()
                        .MaxBy(l => l.Key)
                        .Value.ShipType;
                }
                catch { return null; }
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(JObject @event)
        {
            try
            {
                var eventName = @event["event"].ToString();
                switch (eventName)
                {
                    // Generic
                    case "ShipyardSwap":
                    //case "ShipyardBuy":
                    case "LoadGame":
                    case "Loadout":
                        ProcessShipIDEvent(@event);
                        break;

                    case "Location":
                    case "FSDJump":
                    case "Docked":
                        ProcessShipLocationEvent(@event);
                        break;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error in OnNext");
            }
        }

        private void ProcessShipLocationEvent(JObject @event)
        {
            {
                try
                {
                    var starSystem = @event["StarSystem"].ToString();
                    var timestamp = DateTime.Parse(@event["timestamp"].ToString());


                    lock (playerLocations)
                        if (!playerLocations.ContainsKey(timestamp))
                            if (GetPlayerSystem(timestamp) != starSystem)
                                playerLocations.Add(timestamp, starSystem);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error decoding used ship location");
                }
            }
        }

        private void ProcessShipIDEvent(JObject @event)
        {
            try
            {
                var shipId = @event["ShipID"].ToObject<long>();
                var shipType = (@event["Ship"] ?? @event["ShipType"]).ToString();
                var timestamp = DateTime.Parse(@event["timestamp"].ToString());

                lock (playerShipReferences)
                    if (!playerShipReferences.ContainsKey(timestamp))
                        if (GetPlayerShipId(timestamp) != shipId)
                            playerShipReferences.Add(timestamp, new ShipRecord { ShipID = shipId, ShipType = shipType });
            }
            catch (Exception e)
            {
                logger.Error(e, "Error decoding used ship reference");
            }
        }

        private struct ShipRecord
        {
            public long ShipID;
            public string ShipType;
            public override string ToString() => $"{ShipType}-{ShipID}";
        }
    }
}
