namespace DW.ELA.Controller
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Timers;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Utility;
    using NLog;

    public abstract class AbstractBatchSendPlugin<TEvent, TSettings> : IPlugin, IObserver<JournalEvent>, IDisposable
        where TSettings : class, new()
        where TEvent : class
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Timer flushTimer = new Timer();
        private PluginSettingsFacade<TSettings> pluginSettingsFacade;

        protected AbstractBatchSendPlugin(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            flushTimer.AutoReset = false;
            flushTimer.Interval = FlushInterval.TotalMilliseconds;
            flushTimer.Start();
            flushTimer.Elapsed += (o, e) => FlushQueue();
            pluginSettingsFacade = new PluginSettingsFacade<TSettings>(PluginId, GlobalSettings);
        }

        public abstract string PluginName { get; }

        public abstract string PluginId { get; }

        protected CommanderData CurrentCommander { get; private set; }

        protected virtual TimeSpan FlushInterval => TimeSpan.FromSeconds(10);

        protected IEventConverter<TEvent> EventConverter { get; set; }

        protected ISettingsProvider SettingsProvider { get; }

        protected GlobalSettings GlobalSettings
        {
            get => SettingsProvider.Settings;
            set => SettingsProvider.Settings = value;
        }

        protected TSettings PluginSettings
        {
            get => pluginSettingsFacade.Settings;
            set => pluginSettingsFacade.Settings = value;
        }

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

        public virtual void OnNext(JournalEvent @event)
        {
            if (@event is Commander commanderEvent)
            {
                FlushQueue();
                CurrentCommander = new CommanderData(commanderEvent.Name, commanderEvent.FrontierId);
            }

            foreach (var e in EventConverter.Convert(@event))
                EventQueue.Enqueue(e);
        }

        public virtual void OnCompleted() => FlushQueue();

        public virtual void OnSettingsChanged(object o, EventArgs e) => ReloadSettings();

        public virtual void OnError(Exception error)
        {
        }

        public IObserver<JournalEvent> GetLogObserver() => this;

        public void Dispose() => flushTimer.Dispose();

        public class CommanderData
        {
            public readonly string Name;
            public readonly string FrontierID;

            public CommanderData(string commanderName, string commanderFid)
            {
                Name = commanderName;
                FrontierID = commanderFid;
            }

            public override string ToString() => $"{Name}|{FrontierID}";
        }
    }
}
