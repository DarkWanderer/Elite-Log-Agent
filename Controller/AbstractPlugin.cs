using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using DW.ELA.LogModel;
using DW.ELA.Utility;
using Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Timers;

namespace DW.ELA.Controller
{
    public abstract class AbstractPlugin<TSettings> : IPlugin where TSettings : class, new()
    {
        protected readonly List<LogEvent> EventQueue = new List<LogEvent>();
        protected readonly ISettingsProvider SettingsProvider;
        private readonly Timer flushTimer = new Timer();
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected AbstractPlugin(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider;
            flushTimer.AutoReset = true;
            flushTimer.Interval = FlushInterval.TotalMilliseconds;
            flushTimer.Start();
            flushTimer.Elapsed += (o, e) => FlushQueue();
        }

        public abstract string PluginName { get; }
        public abstract string PluginId { get; }

        protected void FlushQueue()
        {
            try
            {
                LogEvent[] events;
                lock (EventQueue)
                {
                    events = EventQueue.ToArray();
                    EventQueue.Clear();
                }
                ProcessEvents(events);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error while processing events");
            }
        }


        public void OnNext(LogEvent @event)
        {
            lock (EventQueue)
                EventQueue.Add(@event);
        }

        public virtual void OnCompleted() => FlushQueue();
        public virtual void OnSettingsChanged(object o, EventArgs e) => ReloadSettings();
        public virtual void OnError(Exception error) { }

        public IObserver<LogEvent> GetLogObserver() => this;
        public abstract AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings);
        public abstract void ProcessEvents(LogEvent[] events);
        public abstract void ReloadSettings();

        protected GlobalSettings GlobalSettings
        {
            get => SettingsProvider.Settings;
            set => SettingsProvider.Settings = value;
        }

        protected TSettings Settings
        {
            get => new PluginSettingsFacade<TSettings>(PluginId, GlobalSettings).Settings;
            set => new PluginSettingsFacade<TSettings>(PluginId, GlobalSettings).Settings = value;
        }

        public virtual TimeSpan FlushInterval => TimeSpan.FromSeconds(10);
    }
}
