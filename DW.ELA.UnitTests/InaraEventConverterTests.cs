using Controller;
using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Plugin.Inara;
using Interfaces;
using NUnit.Framework;

namespace DW.ELA.UnitTests
{
    [TestFixture]
    public class InaraEventConverterTests
    {
        private readonly IPlayerStateHistoryRecorder stateRecorder = new PlayerStateRecorder();
        private readonly InaraEventConverter eventConverter;

        public InaraEventConverterTests() => eventConverter = new InaraEventConverter(stateRecorder);

        [Test]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.LogEvents))]
        public void ShouldNotFailOnEvents(LogEvent e) => eventConverter.Convert(e);
    }

    [TestFixture]
    public class EventRawJsonExtractorTests
    {
        private readonly EventRawJsonExtractor eventConverter = new EventRawJsonExtractor();

        [Test]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.LogEvents))]
        public void ShouldNotFailOnEvents(LogEvent e) => eventConverter.Convert(e);
    }
}
