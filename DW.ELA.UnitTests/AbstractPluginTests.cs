using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Events;
using DW.ELA.Interfaces.Settings;
using Moq;
using MoreLinq;
using NUnit.Framework;

namespace DW.ELA.UnitTests
{
    [TestFixture]
    [Parallelizable]
    public class AbstractPluginTests
    {
        [Test]
        [Parallelizable]
        public void ShouldNotGiveEventsInstantly()
        {
            using var plugin = new TestPlugin(Mock.Of<ISettingsProvider>());
            TestEventSource.TypedLogEvents
                .Where(e => e.GetType() != typeof(Commander))
                .Take(1000)
                .RandomSubset(50)
                .ForEach(e => plugin.OnNext(e));

            CollectionAssert.IsEmpty(plugin.Flushed);
        }

        [Test]
        [Parallelizable]
        public void ShouldFlushEventsAfterTimeout()
        {
            using var plugin = new TestPlugin(Mock.Of<ISettingsProvider>());
            foreach (var @event in TestEventSource.TypedLogEvents.Skip(10).Take(10))
                plugin.OnNext(@event);
            plugin.FlushQueue();
            CollectionAssert.IsNotEmpty(plugin.Flushed);
            Assert.AreEqual(10, plugin.Flushed.Count);
        }

        private class TestPlugin : AbstractBatchSendPlugin<JournalEvent, TestSettings>
        {
            public readonly ConcurrentBag<JournalEvent> Flushed = new ConcurrentBag<JournalEvent>();

            public TestPlugin(ISettingsProvider settingsProvider)
                : base(settingsProvider)
            {
                EventConverter = new IdentityLogConverter();
            }

            public override string PluginName => "TestPlugin";

            public override string PluginId => "TestPlugin";

            public override void FlushEvents(ICollection<JournalEvent> events)
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

        private class IdentityLogConverter : IEventConverter<JournalEvent>
        {
            public IEnumerable<JournalEvent> Convert(JournalEvent @event)
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
