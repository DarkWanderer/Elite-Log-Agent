namespace DW.ELA.Controller
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Timers;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Utility;
    using NLog;

    public abstract class AbstractPlugin<TEvent, TSettings> : IPlugin, IObserver<LogEvent>, IDisposable
        where TSettings : class, new()
        where TEvent : class
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Timer flushTimer = new Timer();

        protected AbstractPlugin(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            flushTimer.AutoReset = false;
            flushTimer.Interval = FlushInterval.TotalMilliseconds;
            flushTimer.Start();
            flushTimer.Elapsed += (o, e) => FlushQueue();
        }

        public abstract string PluginName { get; }

        public abstract string PluginId { get; }

        protected virtual TimeSpan FlushInterval => TimeSpan.FromSeconds(10);

        protected abstract IEventConverter<TEvent> EventConverter { get; }

        protected ISettingsProvider SettingsProvider { get; }

        protected GlobalSettings GlobalSettings
        {
            get => SettingsProvider.Settings;
            set => SettingsProvider.Settings = value;
        }

        protected TSettings Settings => new PluginSettingsFacade<TSettings>(PluginId, GlobalSettings).Settings;

        protected ConcurrentQueue<TEvent> EventQueue { get; } = new ConcurrentQueue<TEvent>();

        public abstract AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings);

        public abstract void FlushEvents(ICollection<TEvent> events);

        public abstract void ReloadSettings();

        public void FlushQueue()
        {
            try
            {
                var events = new List<TEvent>();
                while (EventQueue.TryDequeue(out var @event))
                    events.Add(@event);
                if (events.Count > 0)
                    FlushEvents(events);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while processing events");
            }
            finally
            {
                flushTimer.Start();
            }
        }

        public virtual void OnNext(LogEvent @event)
        {
            foreach (var e in EventConverter.Convert(@event))
                EventQueue.Enqueue(e);
        }

        public virtual void OnCompleted() => FlushQueue();

        public virtual void OnSettingsChanged(object o, EventArgs e) => ReloadSettings();

        public virtual void OnError(Exception error)
        {
        }

        public IObserver<LogEvent> GetLogObserver() => this;

        public void Dispose() => flushTimer.Dispose();
    }
}
