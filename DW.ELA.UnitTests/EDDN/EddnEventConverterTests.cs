namespace DW.ELA.UnitTests
{
    using System;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.Plugin.EDDN;
    using DW.ELA.Plugin.EDDN.Model;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class EddnEventConverterTests
    {
        [Test]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void ShouldNotFailOnEvents(LogEvent e)
        {
            var mockRecorder = new Mock<IPlayerStateHistoryRecorder>();
            mockRecorder.Setup(r => r.GetStarPos(It.IsAny<string>())).Returns(new[] { 0.0, 1.1, 2.2 });
            mockRecorder.Setup(r => r.GetPlayerSystem(It.IsAny<DateTime>())).Returns("SomeSystem");

            var eventConverter = new EddnEventConverter(mockRecorder.Object) { MaxAge = TimeSpan.FromDays(5000) };
            var result = eventConverter.Convert(e).ToList();
            Assert.NotNull(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(EddnEvent));
            var validator = new EventSchemaValidator();
            foreach (var @event in result)
                Assert.IsTrue(validator.ValidateSchema(@event), "Event {0} should have validated", e.Event);
        }
    }
}