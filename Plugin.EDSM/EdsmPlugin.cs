namespace DW.ELA.Plugin.EDSM
{
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

    public class EdsmPlugin : AbstractBatchSendPlugin<JObject, EdsmSettings>
    {
        private const string EdsmApiUrl = "https://www.edsm.net/api-journal-v1/";
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Task<HashSet<string>> ignoredEvents;
        private readonly ConcurrentDictionary<string, string> ApiKeys = new ConcurrentDictionary<string, string>();

        public EdsmPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder, IRestClientFactory restClientFactory)
            : base(settingsProvider)
        {
            RestClient = restClientFactory.CreateRestClient(EdsmApiUrl);
            EventConverter = new EdsmEventConverter(playerStateRecorder);
            ignoredEvents =
                 RestClient.GetAsync("discard")
                    .ContinueWith((t) => t.IsFaulted ? new HashSet<string>() : new HashSet<string>(JArray.Parse(t.Result).ToObject<string[]>()));

            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
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
            var config = PluginSettings.ApiKeys.ToDictionary();

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0612 // Type or member is obsolete
            string legacyCmdrName = GlobalSettings.CommanderName;
            string legacyApiKey = PluginSettings.ApiKey;
#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete

            if (!string.IsNullOrEmpty(legacyCmdrName) && !string.IsNullOrEmpty(legacyCmdrName) && !config.ContainsKey(legacyCmdrName))
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
                        .TakeLast(1000) // Limit to last N events to avoid EDSM overload
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
                        .Message("Events discarded, commander not known")
                        .Property("eventsCount", events.Count)
                        .Property("commander", commander?.Name ?? "null")
                        .Write();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while processing events");
            }
        }

        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new MultiCmdrApiKeyControl()
        {
            ApiKeys = GetActualApiKeys(),
            GlobalSettings = settings,
            ValidateApiKeyFunc = ValidateApiKeyAsync,
            SaveSettingsFunc = SaveSettings
        };

        private void SaveSettings(GlobalSettings settings, IReadOnlyDictionary<string, string> values) => new PluginSettingsFacade<EdsmSettings>(PluginId, settings).Settings = new EdsmSettings() { ApiKeys = values.ToDictionary() };

        private Task<bool> ValidateApiKeyAsync(string cmdrName, string apiKey) => Task.FromResult(true);
    }
}
