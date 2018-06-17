using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using ELA.Plugin.EDDN;
using Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Utility;

namespace ELA.Plugin.EDDN
{
    public class EddnPlugin : IPlugin, IObserver<JObject>
    {
        private static readonly IRestClient RestClient = new ThrottlingRestClient("https://eddn.edcd.io:4430/upload/");
        private readonly ISettingsProvider settingsProvider;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private readonly List<JObject> eventQueue = new List<JObject>();
        private readonly Timer logFlushTimer = new Timer();
        private IEddnApiFacade apiFacade;
        
        public EddnPlugin(ISettingsProvider settingsProvider, IPlayerStateHistoryRecorder playerStateRecorder)
        {
            this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
            logFlushTimer.AutoReset = true;
            logFlushTimer.Interval = 5000; // send data every n seconds
            logFlushTimer.Elapsed += (o, e) => Task.Factory.StartNew(FlushQueue);
            logFlushTimer.Enabled = true;
            settingsProvider.SettingsChanged += (o, e) => ReloadSettings();
            ReloadSettings();
        }

        private void ReloadSettings()
        {
            FlushQueue();
            apiFacade = new EddnApiFacade(RestClient, GlobalSettings.CommanderName);
        }

        private async void FlushQueue()
        {
            JObject[] apiEvents;
            lock (eventQueue)
            {
                apiEvents = eventQueue.ToArray();
                eventQueue.Clear();
            }
            //if (apiEvents.Length > 0)
            //    await apiFacade?.PostLogEvents(apiEvents);
        }

        public const string CPluginId = "EddnUploader";
        public string PluginName => "EDDN Upload";
        public string PluginId => CPluginId;

        public IObserver<JObject> GetLogObserver() => this;
        public AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => null;

        public void OnCompleted() => FlushQueue();
        public void OnSettingsChanged(object o, EventArgs e) => ReloadSettings();
        public void OnError(Exception error) { }

        public void OnNext(JObject @event)
        {
            return;
        }

        internal GlobalSettings GlobalSettings => settingsProvider.Settings;

        internal EddnSettings Settings
        {
            get => new PluginSettingsFacade<EddnSettings>(PluginId, GlobalSettings).Settings;
            set => new PluginSettingsFacade<EddnSettings>(PluginId, GlobalSettings).Settings = value;
        }
    }
}
