using DW.ELA.Controller;
using DW.ELA.Interfaces;
using DW.ELA.Plugin.EDSM;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DW.ELA.UnitTests.EDSM
{
    [TestFixture]
    [Parallelizable]
    public class EdsmEventConverterTests
    {
        private readonly IPlayerStateHistoryRecorder stateRecorder = new PlayerStateRecorder();
        private readonly EdsmEventConverter eventConverter;

        public EdsmEventConverterTests()
        {
            eventConverter = new EdsmEventConverter(stateRecorder);
        }

        [Test]
        [Parallelizable]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void JsonExtractorShouldNotFailOnEvents(JournalEvent e)
        {
            var result = eventConverter.Convert(e);
            Assert.NotNull(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(JObject));
        }
    }
}
