namespace DW.ELA.UnitTests
{
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable]
    public class EventRawJsonExtractorTests
    {
        private readonly EventRawJsonExtractor eventConverter = new EventRawJsonExtractor();

        [Test]
        [Parallelizable]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void ShouldNotFailOnEvents(LogEvent e)
        {
            var result = eventConverter.Convert(e);
            Assert.NotNull(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(JObject));
        }
    }
}
