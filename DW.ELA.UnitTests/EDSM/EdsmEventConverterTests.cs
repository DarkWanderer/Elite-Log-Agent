namespace DW.ELA.UnitTests.EDSM
{
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Plugin.EDSM;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable]
    public class EdsmEventConverterTests
    {
        private readonly IPlayerStateHistoryRecorder stateRecorder = new PlayerStateRecorder();
        private readonly EdsmEventConverter eventConverter;

        public EdsmEventConverterTests() => eventConverter = new EdsmEventConverter(stateRecorder);

        [Test]
        [Parallelizable]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void JsonExtractorShouldNotFailOnEvents(LogEvent e)
        {
            var result = eventConverter.Convert(e);
            Assert.NotNull(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(JObject));
        }
    }
}
