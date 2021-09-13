using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.UnitTests.Utility;
using DW.ELA.Utility.Json;
using NUnit.Framework;
using MoreLinq;
using DW.ELA.Interfaces.Events;

namespace DW.ELA.UnitTests.Controller
{
    [TestFixture]
    public class JournalMonitorTests
    {
        private static IEnumerable<string> EventsAsJson => TestEventSource.TypedLogEvents
            .OfType<FsdJump>()
            .Cast<JournalEvent>()
            .Select(Serialize.ToJson);

        private Task Delay => Task.Delay(50);

        [Test]
        public async Task ShouldPickUpEvents()
        {
            var directoryProvider = new TestDirectoryProvider();
            var events = new ConcurrentBag<JournalEvent>();
            CollectionAssert.IsEmpty(events);

            string testFile1 = Path.Combine(directoryProvider.Directory, "Journal.1234.log");
            string testFile2 = Path.Combine(directoryProvider.Directory, "Journal.2345.log");

            File.WriteAllText(testFile1, EventsAsJson.ElementAt(0));
            var journalMonitor = new JournalMonitor(directoryProvider, 5);
            journalMonitor.Subscribe(events.Add);

            File.AppendAllText(testFile1, EventsAsJson.ElementAt(1));
            await Delay;
            CollectionAssert.IsNotEmpty(events);

            while (!events.IsEmpty)
                events.TryTake(out var e);

            File.WriteAllText(testFile2, EventsAsJson.ElementAt(2));
            await Delay;
            CollectionAssert.IsNotEmpty(events);

            while (!events.IsEmpty)
                events.TryTake(out var e);

            await Delay;
            CollectionAssert.IsEmpty(events);
        }

        [Test]
        public void ShouldNotFailOnMissingDirectory()
        {
            var directoryProvider = new TestDirectoryProvider();
            Directory.Delete(directoryProvider.Directory, true);
            Assert.IsFalse(Directory.Exists(directoryProvider.Directory));
            Assert.DoesNotThrow(() => new JournalMonitor(directoryProvider, 1));

            // In current implementation, JournalMonitor creates the dir if missing
            // Remove if implementation changes
            Assert.IsTrue(Directory.Exists(directoryProvider.Directory));
        }
    }
}
