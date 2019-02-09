namespace DW.ELA.Plugin.EDDN
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Plugin.EDDN.Model;
    using DW.ELA.Utility;
    using Newtonsoft.Json.Linq;
    using NLog;

    public class EddnPlugin : IPlugin, IObserver<LogEvent>
    {
        private const string EddnUrl = @"https://eddn.edcd.io:4430/upload/";
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;

        private readonly IEddnApiFacade apiFacade;
        private readonly EddnEventConverter eventConverter;
        private readonly EventSchemaValidator schemaManager = new EventSchemaValidator();
        private readonly ConcurrentQueue<JObject> lastPushedEvents = new ConcurrentQueue<JObject>(); // stores last few events to check duplicates
        private readonly ISettingsProvider settingsProvider;

        public EddnPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder, IRestClientFactory restClientFactory)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
            eventConverter = new EddnEventConverter(playerStateRecorder) { UploaderID = settingsProvider.Settings.CommanderName };
            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            apiFacade = new EddnApiFacade(restClientFactory.CreateRestClient(EddnUrl));
            ReloadSettings();
        }

        public string PluginName => "EDDN";

        public string PluginId => "EDDN";

        public void FlushQueue()
        {
        }

        public IObserver<LogEvent> GetLogObserver() => this;

        public AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new EddnSettingsControl();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(LogEvent @event)
        {
            try
            {
                var convertedEvents = eventConverter.Convert(@event);
                foreach (var ce in convertedEvents.Where(IsUnique))
                {
                    var task = apiFacade.PostEventAsync(ce);
                    ContinueWithErrorHandler(task);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while processing event {0}", @event.Event);
            }
        }

        public void OnSettingsChanged(object sender, EventArgs e) => ReloadSettings();

        public void ReloadSettings() => eventConverter.UploaderID = settingsProvider.Settings.CommanderName;

        private void ContinueWithErrorHandler(Task task)
        {
            task.ContinueWith(t => Log.Error(t.Exception, "Error while uploading"), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => Log.Debug("Upload task completed"), TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => Log.Debug("Upload task cancelled"), TaskContinuationOptions.OnlyOnCanceled);
        }

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
                while (lastPushedEvents.Count > 30)
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
                Log.Error(ex, "Error in deduplication");
                return true; // by default, we consider any message unique and so will send it
            }
        }
    }
}
