using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DW.ELA.UnitTests
{
    public class AbstractPluginTests
    {
        [Test]
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
        public void ShouldFlushEventsAfterTimeout()
        {
            using (var plugin = new TestPlugin(Mock.Of<ISettingsProvider>()))
            {
                foreach (var @event in TestEventSource.TypedLogEvents.Skip(10).Take(10))
                    plugin.OnNext(@event);
                Task.Delay(20).Wait();
                CollectionAssert.IsNotEmpty(plugin.Flushed);
                Assert.AreEqual(10, plugin.Flushed.Count);
            }
        }

        private class TestPlugin : AbstractPlugin<LogEvent, TestSettings>
        {
            public readonly ConcurrentBag<LogEvent> Flushed = new ConcurrentBag<LogEvent>();

            public TestPlugin(ISettingsProvider settingsProvider) : base(settingsProvider)
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
            public override void ReloadSettings() { }
            public override TimeSpan FlushInterval => TimeSpan.FromMilliseconds(10);
        }

        private class IdentityLogConverter : IEventConverter<LogEvent>
        {
            public IEnumerable<LogEvent> Convert(LogEvent @event) { yield return @event; }
        }

        private class TestSettings
        {
            bool TestValue { get; set; }
        }
    }

}
