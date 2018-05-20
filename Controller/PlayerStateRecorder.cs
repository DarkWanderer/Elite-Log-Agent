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
        private readonly ILogger Log;

        private readonly SortedList<DateTime, string> playerLocations = new SortedList<DateTime, string>();
        private readonly SortedList<DateTime, long> playerShipReferences = new SortedList<DateTime, long>();

        public PlayerStateRecorder(ILogger log)
        {
            Log = log;
        }

        public string GetPlayerLocation(DateTime atTime)
        {
            try
            {
                return playerLocations.Where(l => l.Key < atTime).MaxBy(l => l.Key).Value;
            }
            catch { return null; }
        }

        public long? GetPlayerShipId(DateTime atTime)
        {
            try
            {
                return playerShipReferences
                    .Where(l => l.Key < atTime)
                    .DefaultIfEmpty()
                    .MaxBy(l => l.Key)
                    .Value;
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
                    case "LoadGame":
                        ProcessShipIDEvent(@event); break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in OnNext");
            }
        }

        private void ProcessShipIDEvent(JObject @event)
        {
            try
            {
                var shipId = @event["ShipID"].ToObject<long>();
                var timestamp = DateTime.Parse(@event["timestamp"].ToString());

                lock (playerShipReferences)
                    if (!playerShipReferences.ContainsKey(timestamp))
                        if (GetPlayerShipId(timestamp) != shipId)
                            playerShipReferences.Add(timestamp, shipId);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error decoding used ship reference");
            }
        }
    }
}
