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
    using DW.ELA.Utility.Extensions;
    using MoreLinq;
    using Newtonsoft.Json.Linq;
    using NLog;
    using NLog.Fluent;

    public class EdsmPlugin : AbstractBatchSendPlugin<JObject, EdsmSettings>
    {
        private const string EdsmApiUrl = "https://www.edsm.net/api-journal-v1/";
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Task<HashSet<string>> ignoredEvents;
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private readonly ConcurrentDictionary<string, string> ApiKeys = new ConcurrentDictionary<string, string>();

        public EdsmPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder, IRestClientFactory restClientFactory)
            : base(settingsProvider)
        {
            RestClient = restClientFactory.CreateRestClient(EdsmApiUrl);
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
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

        public override void ReloadSettings()
        {
            FlushQueue();
            var config = PluginConfiguration.ApiKeys;

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0612 // Type or member is obsolete
            var legacyCmdrName = GlobalSettings.CommanderName;
            var legacyApiKey = PluginConfiguration.ApiKey;
#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete

            if (!string.IsNullOrEmpty(legacyCmdrName) && !string.IsNullOrEmpty(legacyCmdrName) && !config.ContainsKey(legacyCmdrName))
                config.Add(legacyCmdrName, legacyApiKey);

            // Update keys for which new values were provided
            foreach (var kvp in config)
                ApiKeys.AddOrUpdate(kvp.Key, kvp.Value, (key, oldValue) => kvp.Value);

            // Remove keys which were removed from config
            foreach (var key in ApiKeys.Keys.Except(config.Keys))
                ApiKeys.TryRemove(key, out var _);
        }

        public override async void FlushEvents(ICollection<JObject> events)
        {
            try
            {
                var commander = CurrentCommander;
                if (commander != null && ApiKeys.TryGetValue(commander.Name, out var apiKey))
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
                        .Message("Uploaded {0} events", events.Count)
                        .Property("eventsCount", events.Count)
                        .Write();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while processing events");
            }
        }

        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new MultiCmdrApiKeyControl() { GlobalSettings = settings };
    }
}
