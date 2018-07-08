using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using DW.ELA.LogModel;
using DW.ELA.Plugin.Inara.Model;
using DW.ELA.Utility.Extensions;
using Interfaces;
using MoreLinq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DW.ELA.Plugin.Inara
{
    public class InaraPlugin : AbstractPlugin<ApiEvent,InaraSettings>
    {
        public override string PluginName => CPluginName;
        public override string PluginId => CPluginId;

        public const string CPluginName = "INARA settings";
        public const string CPluginId = "InaraUploader";
        public static readonly IRestClient RestClient = new ThrottlingRestClient("https://inara.cz/inapi/v1/");

        private readonly InaraEventConverter eventConverter;
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private ISettingsProvider settingsProvider;
        protected override IEventConverter<ApiEvent> EventConverter => eventConverter;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public InaraPlugin(IPlayerStateHistoryRecorder playerStateRecorder, ISettingsProvider settingsProvider) : base(settingsProvider)
        {
            this.playerStateRecorder = playerStateRecorder;
            eventConverter = new InaraEventConverter(this.playerStateRecorder);
            this.settingsProvider = settingsProvider;
            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        public override void ReloadSettings() => FlushQueue();
        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new InaraSettingsControl() { GlobalSettings = settings };
        public override void OnSettingsChanged(object o, EventArgs e) => ReloadSettings();

        public override async void FlushEvents(ICollection<ApiEvent> events)
        {
            if (!Settings.Verified)
                return;
            try
            {
                var facade = new InaraApiFacade(RestClient, Settings.ApiKey, GlobalSettings.CommanderName);
                var apiEvents = Compact(events).ToArray();
                if (apiEvents.Length > 0)
                    await facade.ApiCall(apiEvents);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error while processing events");
            }
        }

        private static readonly string[] latestOnlyEvents = new[] {
            "setCommanderInventoryMaterials",
            "setCommanderGameStatistics",
            "setCommanderStorageModules"
        };

        private static readonly IReadOnlyDictionary<string, string[]> supersedesEvents = new Dictionary<string, string[]>
        {
            { "setCommanderInventoryMaterials", new[]{"addCommanderInventoryMaterialsItem" } }
        };

        private static IEnumerable<ApiEvent> Compact(IEnumerable<ApiEvent> events)
        {
            var eventsByType = events
                .GroupBy(e => e.EventName, e => e)
                .ToDictionary(g => g.Key, g => g.ToArray());
            foreach (var type in latestOnlyEvents.Intersect(eventsByType.Keys))
                eventsByType[type] = new[] { eventsByType[type].MaxBy(e => e.Timestamp).FirstOrDefault() };

            // It does not make sense to e.g. add inventory materials if we already have a newer inventory snapshot
            foreach (var type in supersedesEvents.Keys.Intersect(eventsByType.Keys))
            {
                var cutoffTimestamp = eventsByType[type].Max(e => e.Timestamp);
                foreach (var supersededType in supersedesEvents[type].Intersect(eventsByType.Keys))
                    eventsByType[supersededType] = eventsByType[supersededType]
                        .Where(e => e.Timestamp > cutoffTimestamp)
                        .ToArray();
            }

            return eventsByType.Values.SelectMany(ev => ev).OrderBy(e => e.Timestamp);
        }
    }
}
