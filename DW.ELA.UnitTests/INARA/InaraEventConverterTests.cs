namespace DW.ELA.UnitTests.INARA
{
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Plugin.Inara;
    using DW.ELA.Plugin.Inara.Model;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable]
    public class InaraEventConverterTests
    {
        private readonly IPlayerStateHistoryRecorder stateRecorder = new PlayerStateRecorder();
        private readonly InaraEventConverter eventConverter;

        public InaraEventConverterTests() => eventConverter = new InaraEventConverter(stateRecorder);

        [Test]
        [Parallelizable]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void InaraConverterShouldNotFailOnEvents(LogEvent e)
        {
            var result = eventConverter.Convert(e);
            Assert.NotNull(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ApiEvent));
        }
    }
}
