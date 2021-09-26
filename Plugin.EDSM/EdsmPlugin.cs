using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using DW.ELA.Utility;
using MoreLinq;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Fluent;

namespace DW.ELA.Plugin.EDSM
{
    public class EdsmPlugin : AbstractBatchSendPlugin<JObject, EdsmSettings>, IApiKeyValidator
    {
        private const string EdsmApiUrl = "https://www.edsm.net/api-journal-v1/";
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Task<HashSet<string>> ignoredEvents;
        private readonly ConcurrentDictionary<string, string> ApiKeys = new();
        private readonly IUserNotificationInterface notificationInterface;

        public EdsmPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder, IRestClientFactory restClientFactory, IUserNotificationInterface notificationInterface)
            : base(settingsProvider)
        {
            RestClient = restClientFactory.CreateRestClient(EdsmApiUrl);
            EventConverter = new EdsmEventConverter(playerStateRecorder);
            ignoredEvents =
                 RestClient.GetAsync("discard")
                    .ContinueWith((t) => t.IsFaulted ? new HashSet<string>() : new HashSet<string>(JArray.Parse(t.Result).ToObject<string[]>()));

            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
            this.notificationInterface = notificationInterface;
        }

        protected internal IRestClient RestClient { get; }

        protected override TimeSpan FlushInterval => TimeSpan.FromMinutes(1);

        public override string PluginName => "EDSM";

        public override string PluginId => "EdsmUploader";

        /// <summary>
        /// Property which merges the 'new' API keys with multi-commander support with the old legacy single-commander one
        /// </summary>
        private IReadOnlyDictionary<string, string> GetActualApiKeys()
        {
            var pluginSettings = SettingsFacade.GetPluginSettings(GlobalSettings);
            var config = pluginSettings.ApiKeys.ToDictionary();
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

        public override async void FlushEvents(ICollection<JObject> events)
        {
            try
            {
                var commander = CurrentCommander;
                if (commander != null && ApiKeys.TryGetValue(commander.Name, out string apiKey))
                {
                    var apiFacade = new EdsmApiFacade(RestClient, commander.Name, apiKey);
                    var apiEventsBatches = events
                        .Where(e => !ignoredEvents.Result.Contains(e["event"].ToString()))
                        .Reverse()
                        .Batch(100) // EDSM API only accepts 100 events in single call
                        .ToList();
                    foreach (var batch in apiEventsBatches)
                        await apiFacade.PostLogEvents(batch.ToArray());

                    Log.Info()
                        .Message("Uploaded events")
                        .Property("eventsCount", events.Count)
                        .Property("commander", commander)
                        .Write();
                }
                else
                {
                    Log.Info()
                        .LoggerName(Log.Name)
                        .Message("No EDSM API key set for commander, events discarded")
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
                Log.Error(e, "Error while processing events for EDSM");
            }
        }

        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new MultiCmdrApiKeyControl()
        {
            ApiKeys = GetActualApiKeys(),
            ApiKeyValidator = this,
            ApiSettingsLink = "https://www.edsm.net/en/settings/api",
            GlobalSettings = settings,
            SaveSettingsFunc = SaveSettings
        };

        private void SaveSettings(GlobalSettings settings, IReadOnlyDictionary<string, string> values) => new PluginSettingsFacade<EdsmSettings>(PluginId).SetPluginSettings(settings, new EdsmSettings() { ApiKeys = values.ToDictionary() });

        public async Task<bool> ValidateKeyAsync(string cmdrName, string apiKey)
        {
            try
            {
                var apiFacade = new EdsmApiFacade(new ThrottlingRestClient.Factory().CreateRestClient("https://www.edsm.net/api-commander-v1/get-ranks"), cmdrName, apiKey);
                var result = await apiFacade.GetCommanderRanks();
                var combatRank = result?["ranksVerbose"]?["Combat"]?.ToString();
                return combatRank != null;
            }
            catch (InvalidApiKeyException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Log.Info(ex, "Exception while validating API key");
                return false;
            }
        }
    }
}
