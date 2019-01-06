namespace DW.ELA.UnitTests
{
    using System;
    using System.Linq;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Plugin.EDDN;
    using DW.ELA.Plugin.EDDN.Model;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class EddnEventConverterTests
    { 
        [Test]
        [Parallelizable]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void ShouldNotFailOnEvents(LogEvent e)
        {
            var recorderMock = GetRecorderMock();

            var eventConverter = new EddnEventConverter(recorderMock) { MaxAge = TimeSpan.FromDays(5000) };
            var result = eventConverter.Convert(e).ToList();
            Assert.NotNull(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(EddnEvent));
            var validator = new EventSchemaValidator();
            foreach (var @event in result)
                Assert.IsTrue(validator.ValidateSchema(@event), "Event {0} should have validated", e.Event);
        }

        [Test]
        [Parallelizable]
        public void ShouldEnrichJournalEventsWithStarSystemFields()
        {
            var recorderMock = GetRecorderMock();

            var eventConverter = new EddnEventConverter(recorderMock) { MaxAge = TimeSpan.FromDays(5000) };

            var convertedEvents = TestEventSource.CannedEvents
                .SelectMany(eventConverter.Convert)
                .OfType<JournalEvent>()
                .ToList();

            CollectionAssert.IsNotEmpty(convertedEvents);
            CollectionAssert.AllItemsAreNotNull(convertedEvents);

            foreach (var e in convertedEvents.OfType<JournalEvent>())
            {
                Assert.NotNull(e.Message.Property("SystemAddress"));
                Assert.NotNull(e.Message.Property("StarPos"));
                Assert.NotNull(e.Message.Property("StarSystem"));
            }
        }

        private IPlayerStateHistoryRecorder GetRecorderMock()
        {
            var recorderMock = new Mock<IPlayerStateHistoryRecorder>();
            recorderMock.Setup(r => r.GetStarPos(It.IsAny<string>())).Returns(new[] { 0.0, 1.1, 2.2 });
            recorderMock.Setup(r => r.GetPlayerSystem(It.IsAny<DateTime>())).Returns("SomeSystem");
            recorderMock.Setup(r => r.GetSystemAddress(It.IsAny<string>())).Returns(123456789456);
            return recorderMock.Object;
        }
    }
}