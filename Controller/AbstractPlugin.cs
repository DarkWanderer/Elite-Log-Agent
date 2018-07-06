using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using DW.ELA.Utility;
using Interfaces;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;

namespace DW.ELA.Controller
{
    public abstract class AbstractPlugin<TEvent, TSettings> : IPlugin
        where TSettings : class, new()
        where TEvent : class
    {
        protected readonly ISettingsProvider SettingsProvider;
        private readonly ConcurrentQueue<TEvent> EventQueue = new ConcurrentQueue<TEvent>();
        private readonly Timer flushTimer = new Timer();

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        protected AbstractPlugin(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider;
            flushTimer.AutoReset = false;
            flushTimer.Interval = FlushInterval.TotalMilliseconds;
            flushTimer.Start();
            flushTimer.Elapsed += (o, e) => FlushQueue();
        }

        public virtual TimeSpan FlushInterval => TimeSpan.FromSeconds(10);

        public abstract string PluginName { get; }
        public abstract string PluginId { get; }

        protected abstract IEventConverter<TEvent> EventConverter { get; }
        public abstract AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings);
        public abstract void FlushEvents(ICollection<TEvent> events);
        public abstract void ReloadSettings();

        protected void FlushQueue()
        {
            try
            {
                var events = new List<TEvent>();
                while (EventQueue.TryDequeue(out var @event))
                    events.Add(@event);
                FlushEvents(events);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error while processing events");
            }
            finally
            {
                flushTimer.Start();
            }
        }

        public void OnNext(LogEvent @event)
        {
            foreach (var e in EventConverter.Convert(@event))
                EventQueue.Enqueue(e);
        }

        public virtual void OnCompleted() => FlushQueue();
        public virtual void OnSettingsChanged(object o, EventArgs e) => ReloadSettings();
        public virtual void OnError(Exception error) { }

        public IObserver<LogEvent> GetLogObserver() => this;

        protected GlobalSettings GlobalSettings
        {
            get => SettingsProvider.Settings;
            set => SettingsProvider.Settings = value;
        }

        protected TSettings Settings => new PluginSettingsFacade<TSettings>(PluginId, GlobalSettings).Settings;
    }
}
