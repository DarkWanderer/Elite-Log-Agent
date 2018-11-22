namespace DW.ELA.UnitTests
{
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class EventRawJsonExtractorTests
    {
        private readonly EventRawJsonExtractor eventConverter = new EventRawJsonExtractor();

        [Test]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void ShouldNotFailOnEvents(LogEvent e)
        {
            var result = eventConverter.Convert(e);
            Assert.NotNull(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(JObject));
        }
    }
}
