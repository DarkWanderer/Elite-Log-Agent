using System;
using System.Collections.Generic;
using System.Linq;
using DW.ELA.Interfaces;
using DW.ELA.LogModel.Events;
using Newtonsoft.Json.Linq;
using NLog;
using Utility;

namespace DW.ELA.Plugin.EDDN
{
    public class EddnEventConverter : IEventConverter<EddnEvent>
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public string UploaderID = "Unknown";

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
            try
            {
                switch (@event)
                {
                    //case Docked e: return MakeJournalEvent(e);
                    case FsdJump e: return MakeJournalEvent(e);
                    //case Scan e: return MakeJournalEvent(e);
                    case Location e: return MakeJournalEvent(e);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error converting message");
            }
            return Enumerable.Empty<EddnEvent>();
        }

        private IEnumerable<EddnEvent> MakeJournalEvent(Location e) { yield return new JournalEvent { Header = CreateHeader(), Message = Strip(e.Raw) }; }
        private IEnumerable<EddnEvent> MakeJournalEvent(Scan e) { yield return new JournalEvent { Header = CreateHeader(), Message = Strip(e.Raw) }; }
        private IEnumerable<EddnEvent> MakeJournalEvent(FsdJump e) { yield return new JournalEvent { Header = CreateHeader(), Message = Strip(e.Raw) }; }
        private IEnumerable<EddnEvent> MakeJournalEvent(Docked e) { yield return new JournalEvent { Header = CreateHeader(), Message = Strip(e.Raw) }; }

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
            "Longitude"};

            foreach (var attribute in raw)
                if (attribute.Key.EndsWith("_Localised"))
                    attributesToRemove.Add(attribute.Key);

            foreach (var key in attributesToRemove)
                raw.Remove(key);

            return raw;
        }
    }
}