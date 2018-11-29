namespace DW.ELA.Plugin.EDDN
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Plugin.EDDN.Model;
    using DW.ELA.Utility;
    using Newtonsoft.Json.Linq;
    using NLog;

    public class EddnPlugin : AbstractPlugin<EddnEvent, EddnSettings>
    {
        private static readonly IRestClient RestClient = new ThrottlingRestClient("https://eddn.edcd.io:4430/upload/");
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider settingsProvider;
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;

        private readonly IEddnApiFacade apiFacade = new EddnApiFacade(RestClient);
        private readonly EddnEventConverter eventConverter;
        private readonly EventSchemaValidator schemaManager = new EventSchemaValidator();

        public EddnPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder)
            : base(settingsProvider)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
            eventConverter = new EddnEventConverter(playerStateRecorder) { UploaderID = settingsProvider.Settings.CommanderName };
            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        protected override IEventConverter<EddnEvent> EventConverter => eventConverter;

        public override string PluginName => "EDDN";

        public override string PluginId => "EDDN";

        // EDDN accepts events one-by-one, so no need to batch
        public override TimeSpan FlushInterval => TimeSpan.FromSeconds(1);

        public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => null;

        public override async void FlushEvents(ICollection<EddnEvent> events)
        {
            foreach (var @event in events)
                schemaManager.ValidateSchema(@event);
            await apiFacade.PostEventsAsync(events.Where(IsUnique).ToArray());
        }

        private readonly ConcurrentQueue<JObject> lastPushedEvents = new ConcurrentQueue<JObject>(); // stores

        /// <summary>
        /// Check event against list of last few sent events, excluding timestamp from comparison
        /// </summary>
        /// <param name="e">Event to check uniqueness</param>
        /// <returns>true if event wasn't sent before</returns>
        private bool IsUnique(EddnEvent e)
        {
            try
            {
                JObject jObject;
                while (lastPushedEvents.Count > 10)
                    lastPushedEvents.TryDequeue(out jObject);

                jObject = JObject.FromObject(e);
                var messageObject = jObject.Property("message")?.Value as JObject;
                messageObject?.Property("timestamp")?.Remove();

                foreach (var recent in lastPushedEvents)
                {
                    if (JToken.DeepEquals(jObject, recent))
                        return false;
                }

                lastPushedEvents.Enqueue(jObject);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in deduplication");
                return true; // by default, we consider any message unique and so will send it
            }
        }

        public override void ReloadSettings() => eventConverter.UploaderID = settingsProvider.Settings.CommanderName;
    }
}
