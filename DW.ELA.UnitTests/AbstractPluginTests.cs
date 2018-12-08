namespace DW.ELA.UnitTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Settings;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable]
    public class AbstractPluginTests
    {
        [Test]
        [Parallelizable]
        public void ShouldNotGiveEventsInstantly()
        {
            using (var plugin = new TestPlugin(Mock.Of<ISettingsProvider>()))
            {
                foreach (var @event in TestEventSource.TypedLogEvents.Skip(10).Take(10))
                    plugin.OnNext(@event);
                CollectionAssert.IsEmpty(plugin.Flushed);
            }
        }

        [Test]
        [Parallelizable]
        public void ShouldFlushEventsAfterTimeout()
        {
            using (var plugin = new TestPlugin(Mock.Of<ISettingsProvider>()))
            {
                foreach (var @event in TestEventSource.TypedLogEvents.Skip(10).Take(10))
                    plugin.OnNext(@event);
                plugin.FlushQueue();
                CollectionAssert.IsNotEmpty(plugin.Flushed);
                Assert.AreEqual(10, plugin.Flushed.Count);
            }
        }

        private class TestPlugin : AbstractPlugin<LogEvent, TestSettings>
        {
            public readonly ConcurrentBag<LogEvent> Flushed = new ConcurrentBag<LogEvent>();

            public TestPlugin(ISettingsProvider settingsProvider)
                : base(settingsProvider)
            {
            }

            public override string PluginName => "TestPlugin";

            public override string PluginId => "TestPlugin";

            protected override IEventConverter<LogEvent> EventConverter => new IdentityLogConverter();

            public override void FlushEvents(ICollection<LogEvent> events)
            {
                foreach (var e in events)
                    Flushed.Add(e);
            }

            public override AbstractSettingsControl GetPluginSettingsControl(GlobalSettings settings) => null;

            public override void ReloadSettings()
            {
            }

            public new void FlushQueue() => base.FlushQueue();

            protected override TimeSpan FlushInterval => TimeSpan.FromHours(10);
        }

        private class IdentityLogConverter : IEventConverter<LogEvent>
        {
            public IEnumerable<LogEvent> Convert(LogEvent @event)
            {
                yield return @event;
            }
        }

        private class TestSettings
        {
            public bool TestValue { get; set; }
        }
    }
}
