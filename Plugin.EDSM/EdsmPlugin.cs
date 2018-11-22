namespace ELA.Plugin.EDSM
{
    using System;
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

    public class EdsmPlugin : AbstractPlugin<JObject, EdsmSettings>
    {
        private readonly IRestClient restClient = new ThrottlingRestClient("https://www.edsm.net/api-journal-v1/");
        private Task<HashSet<string>> ignoredEvents;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private IEdsmApiFacade apiFacade;
        private readonly IEventConverter<JObject> eventConverter = new EventRawJsonExtractor();

        protected override IEventConverter<JObject> EventConverter => eventConverter;

        public override TimeSpan FlushInterval => TimeSpan.FromMinutes(1);

        public EdsmPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder)
            : base(settingsProvider)
        {
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));

            ignoredEvents =
                 restClient.GetAsync("discard")
                    .ContinueWith((t) => new HashSet<string>(JArray.Parse(t.Result).ToObject<string[]>()));

            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        public override void ReloadSettings()
        {
            FlushQueue();
            apiFacade = new EdsmApiFacade(restClient, GlobalSettings.CommanderName, Settings.ApiKey);
        }

        public override async void FlushEvents(ICollection<JObject> events)
        {
            if (!Settings.Verified)
                return;
            try
            {
                var apiEventsBatches = events
                    .TakeLast(2000) // Limit to last N events to avoid EDSM overload
                    .Reverse()
                    .Select(Enrich)
                    .Batch(100) // EDSM API only accepts 100 events in single call
                    .ToList();
                foreach (var batch in apiEventsBatches)
                    await apiFacade?.PostLogEvents(batch.ToArray());
                logger.Info("Uploaded {0} EDSM events", events.Count);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error while processing events");
            }
        }

        public const string CPluginId = "EdsmUploader";

        public override string PluginName => "EDSM";

        public override string PluginId => CPluginId;

        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new EdsmSettingsControl() { GlobalSettings = settings };

        private JObject Enrich(JObject @event)
        {
            @event = (JObject)@event.DeepClone();
            var timestamp = DateTime.Parse(@event["timestamp"].ToString());
            @event["_systemName"] = playerStateRecorder.GetPlayerSystem(timestamp);
            @event["_shipId"] = playerStateRecorder.GetPlayerShipId(timestamp);
            return @event;
        }
    }
}
