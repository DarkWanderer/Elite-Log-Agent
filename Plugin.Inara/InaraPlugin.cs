using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using DW.ELA.Plugin.Inara.Model;
using DW.ELA.Utility;
using MoreLinq;
using NLog;
using NLog.Fluent;

namespace DW.ELA.Plugin.Inara
{
    public class InaraPlugin : AbstractBatchSendPlugin<ApiInputEvent, InaraSettings>, IApiKeyValidator
    {
        public const string CPluginName = "INARA";
        public const string CPluginId = "InaraUploader";
        private const string InaraApiUrl = "https://inara.cz/inapi/v1/";
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private readonly IUserNotificationInterface notificationInterface;
        private readonly ConcurrentDictionary<string, string> ApiKeys = new ConcurrentDictionary<string, string>();

        public InaraPlugin(IPlayerStateHistoryRecorder playerStateRecorder, ISettingsProvider settingsProvider, IRestClientFactory restClientFactory, IUserNotificationInterface notificationInterface)
            : base(settingsProvider)
        {
            RestClient = restClientFactory.CreateRestClient(InaraApiUrl);
            this.playerStateRecorder = playerStateRecorder;
            this.notificationInterface = notificationInterface;
            EventConverter = new InaraEventConverter(this.playerStateRecorder);
            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        public override string PluginName => CPluginName;

        public override string PluginId => CPluginId;

        protected internal IRestClient RestClient { get; }

        // Explicitly set to 30 as Inara prefers batches of events
        protected override TimeSpan FlushInterval => TimeSpan.FromSeconds(30);

        /// <summary>
        /// Property which merges the 'new' API keys with multi-commander support with the old legacy single-commander one
        /// </summary>
        private IReadOnlyDictionary<string, string> GetActualApiKeys()
        {
            var pluginSettings = SettingsFacade.GetPluginSettings(GlobalSettings);
            var config = pluginSettings.ApiKeys.ToDictionary();

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0612 // Type or member is obsolete
            string legacyCmdrName = GlobalSettings.CommanderName;
            string legacyApiKey = pluginSettings.ApiKey;
#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete

            if (!string.IsNullOrEmpty(legacyCmdrName) && !string.IsNullOrEmpty(legacyApiKey) && !config.ContainsKey(legacyCmdrName))
                config.Add(legacyCmdrName, legacyApiKey);

            return config;
        }

        public override void ReloadSettings()
        {
            FlushQueue();
            var actualApiKeys = GetActualApiKeys();

            // Update keys for which new values were provided
            foreach (var kvp in actualApiKeys)
                ApiKeys.AddOrUpdate(kvp.Key, kvp.Value, (key, oldValue) => kvp.Value);

            // Remove keys which were removed from config
            foreach (string key in ApiKeys.Keys.Except(actualApiKeys.Keys))
                ApiKeys.TryRemove(key, out string _);
        }


        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new MultiCmdrApiKeyControl()
        {
            ApiKeys = GetActualApiKeys(),
            ApiKeyValidator = this,
            ApiSettingsLink = "https://inara.cz/settings-api/",
            GlobalSettings = settings,
            SaveSettingsFunc = SaveSettings
        };

        private void SaveSettings(GlobalSettings settings, IReadOnlyDictionary<string, string> values) =>
            new PluginSettingsFacade<InaraSettings>(PluginId).SetPluginSettings(settings, new InaraSettings() { ApiKeys = values.ToDictionary() });

        public override void OnSettingsChanged(object o, EventArgs e) => ReloadSettings();

        public override async void FlushEvents(ICollection<ApiInputEvent> events)
        {
            try
            {
                var commander = CurrentCommander;
                if (commander != null && ApiKeys.TryGetValue(commander.Name, out string apiKey))
                {
                    var facade = new InaraApiFacade(RestClient, commander.Name, apiKey, commander.FrontierID);
                    var ApiInputs = Compact(events).ToArray();
                    if (ApiInputs.Length > 0)
                        await facade.ApiCall(ApiInputs);

                    Log.Info()
                        .LoggerName(Log.Name)
                        .Message("Uploaded events")
                        .Property("eventsCount", events.Count)
                        .Property("commander", commander)
                        .Write();
                }
                else
                {
                    Log.Info()
                        .LoggerName(Log.Name)
                        .Message("No INARA API key set for commander, events discarded")
                        .Property("eventsCount", events.Count)
                        .Property("commander", commander?.Name ?? "null")
                        .Write();
                }
            }
            catch (InvalidApiKeyException)
            {
                notificationInterface.ShowErrorNotification("Invalid EDSM API key for CMDR " + CurrentCommander?.Name);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while processing events for INARA");
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

        private static IEnumerable<ApiInputEvent> Compact(IEnumerable<ApiInputEvent> events)
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

        public async Task<bool> ValidateKeyAsync(string cmdrName, string apiKey)
        {
            try
            {
                var apiFacade = new InaraApiFacade(RestClient, cmdrName, apiKey);
                return await apiFacade.GetCmdrName() == cmdrName;
            }
            catch (InvalidApiKeyException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Log.Warn(ex, "Exception while validating API key");
                return false;
            }
        }
    }
}
