namespace DW.ELA.Plugin.Inara
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Plugin.Inara.Model;
    using MoreLinq;
    using NLog;

    public class InaraPlugin : AbstractPlugin<ApiEvent, InaraSettings>
    {
        public const string CPluginName = "INARA";
        public const string CPluginId = "InaraUploader";
        private const string InaraApiUrl = "https://inara.cz/inapi/v1/";
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly InaraEventConverter eventConverter;
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private readonly ISettingsProvider settingsProvider;

        public InaraPlugin(IPlayerStateHistoryRecorder playerStateRecorder, ISettingsProvider settingsProvider, IRestClientFactory restClientFactory)
            : base(settingsProvider)
        {
            RestClient = restClientFactory.CreateRestClient(InaraApiUrl);
            this.playerStateRecorder = playerStateRecorder;
            eventConverter = new InaraEventConverter(this.playerStateRecorder);
            this.settingsProvider = settingsProvider;
            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        public override string PluginName => CPluginName;

        public override string PluginId => CPluginId;

        protected internal IRestClient RestClient { get; }

        protected override IEventConverter<ApiEvent> EventConverter => eventConverter;

        // Explicitly set to 30 as Inara prefers batches of events
        protected override TimeSpan FlushInterval => TimeSpan.FromSeconds(30);

        public override void ReloadSettings() => FlushQueue();

        public override void OnNext(LogEvent @event)
        {
            base.OnNext(@event);
            if (Settings.ManageFriends && @event is Friends friends)
            {
                foreach (var e in eventConverter.ConvertFriendsEvent(friends))
                    EventQueue.Enqueue(e);
            }
        }

        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new InaraSettingsControl() { GlobalSettings = settings, RestClient = RestClient };

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
                Log.Error(e, "Error while processing events");
            }
        }

        private static readonly string[] LatestOnlyEvents = new[]
        {
            "setCommanderInventoryMaterials",
            "setCommanderGameStatistics",
            "setCommanderStorageModules",
            "setCommanderInventoryCargo",
            "setCommanderRankPower"
        };

        private static readonly IReadOnlyDictionary<string, string[]> SupersedesEvents = new Dictionary<string, string[]>
        {
            { "setCommanderInventoryMaterials", new[] { "addCommanderInventoryMaterialsItem", "delCommanderInventoryMaterialsItem" } }
        };

        private static IEnumerable<ApiEvent> Compact(IEnumerable<ApiEvent> events)
        {
            var eventsByType = events
                .GroupBy(e => e.EventName, e => e)
                .ToDictionary(g => g.Key, g => g.ToArray());
            foreach (string type in LatestOnlyEvents.Intersect(eventsByType.Keys))
                eventsByType[type] = new[] { eventsByType[type].MaxBy(e => e.Timestamp).FirstOrDefault() };

            // It does not make sense to e.g. add inventory materials if we already have a newer inventory snapshot
            foreach (string type in SupersedesEvents.Keys.Intersect(eventsByType.Keys))
            {
                var cutoffTimestamp = eventsByType[type].Max(e => e.Timestamp);
                foreach (string supersededType in SupersedesEvents[type].Intersect(eventsByType.Keys))
                {
                    eventsByType[supersededType] = eventsByType[supersededType]
                        .Where(e => e.Timestamp > cutoffTimestamp)
                        .ToArray();
                }
            }

            return eventsByType.Values.SelectMany(ev => ev).OrderBy(e => e.Timestamp);
        }
    }
}
