using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Utility;

namespace ELA.Plugin.EDSM
{
    public class EdsmPlugin : IPlugin, IObserver<JObject>
    {
        private static readonly IRestClient RestClient = new ThrottlingRestClient("https://www.edsm.net/api-journal-v1/");
        private Task<HashSet<string>> ignoredEvents;
        private readonly ISettingsProvider settingsProvider;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private readonly List<JObject> eventQueue = new List<JObject>();
        private readonly Timer logFlushTimer = new Timer();
        private IEdsmApiFacade apiFacade;
        
        public EdsmPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
            logFlushTimer.AutoReset = true;
            logFlushTimer.Interval = 5000; // send data every n seconds
            logFlushTimer.Elapsed += (o, e) => Task.Factory.StartNew(FlushQueue);
            logFlushTimer.Enabled = true;

            ignoredEvents =
                 RestClient.GetAsync("discard")
                    .ContinueWith((t) => new HashSet<string>(JArray.Parse(t.Result).ToObject<string[]>()));

            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        private void ReloadSettings()
        {
            FlushQueue();
            apiFacade = new EdsmApiFacade(RestClient, GlobalSettings.CommanderName, Settings.ApiKey);
        }

        private async void FlushQueue()
        {
            JObject[] apiEvents;
            lock (eventQueue)
            {
                apiEvents = eventQueue.ToArray();
                eventQueue.Clear();
            }
            if (apiEvents.Length > 0)
                await apiFacade?.PostLogEvents(apiEvents);
        }

        public const string CPluginId = "EdsmUploader";
        public string PluginName => "EDSM Upload";
        public string PluginId => CPluginId;

        public IObserver<JObject> GetLogObserver() => this;
        public AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => new EdsmSettingsControl() { GlobalSettings = settings };

        public void OnCompleted() => FlushQueue();
        public void OnSettingsChanged(object o, EventArgs e) => ReloadSettings();
        public void OnError(Exception error) { }

        public void OnNext(JObject @event)
        {
            if (!Settings.Verified)
                return;

            var eventName = @event["event"].ToString();
            if (ignoredEvents.Result.Contains(eventName))
                return;
            @event = (JObject)@event.DeepClone(); // have to clone the object here as we'll have to make modifications to it
            EnrichEvent(@event);
            lock (eventQueue)
            {
                eventQueue.Add(@event);
                if (eventQueue.Count > 1000)
                    Task.Factory.StartNew(FlushQueue);
            }
            logger.Trace("Queued event {0}", @event);
        }

        private void EnrichEvent(JObject @event)
        {
            var timestamp = DateTime.Parse(@event["timestamp"].ToString());
            @event["_systemName"] = playerStateRecorder.GetPlayerSystem(timestamp);
            @event["_shipId"] = playerStateRecorder.GetPlayerShipId(timestamp);
        }


        internal GlobalSettings GlobalSettings => settingsProvider.Settings;

        internal EdsmSettings Settings
        {
            get => new PluginSettingsFacade<EdsmSettings>(PluginId, GlobalSettings).Settings;
            set => new PluginSettingsFacade<EdsmSettings>(PluginId, GlobalSettings).Settings = value;
        }
    }
}
