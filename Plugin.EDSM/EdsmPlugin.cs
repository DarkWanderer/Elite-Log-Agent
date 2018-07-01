using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using DW.ELA.LogModel;
using Interfaces;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace ELA.Plugin.EDSM
{
    public class EdsmPlugin : AbstractPlugin<JObject,EdsmSettings>
    {
        private readonly IRestClient RestClient = new ThrottlingRestClient("https://www.edsm.net/api-journal-v1/");
        private Task<HashSet<string>> ignoredEvents;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private IEdsmApiFacade apiFacade;
        private readonly IEventConverter<JObject> eventConverter = new EventRawJsonExtractor();
        protected override IEventConverter<JObject> EventConverter => eventConverter;

        public EdsmPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder) : base (settingsProvider)
        {
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));

            ignoredEvents =
                 RestClient.GetAsync("discard")
                    .ContinueWith((t) => new HashSet<string>(JArray.Parse(t.Result).ToObject<string[]>()));

            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        public override void ReloadSettings()
        {
            FlushQueue();
            apiFacade = new EdsmApiFacade(RestClient, GlobalSettings.CommanderName, Settings.ApiKey);
        }

        public override async void FlushEvents(JObject[] events)
        {
            if (!Settings.Verified)
                return;
            try
            {
                var apiEvents = events.Select(Enrich).ToArray();
                if (apiEvents.Length > 0)
                    await apiFacade?.PostLogEvents(apiEvents);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error while processing events");
            }
        }

        public const string CPluginId = "EdsmUploader";
        public override string PluginName => "EDSM Upload";
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
