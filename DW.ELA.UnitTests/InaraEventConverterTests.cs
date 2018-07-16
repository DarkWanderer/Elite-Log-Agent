using Controller;
using DW.ELA.Interfaces;
using DW.ELA.Plugin.Inara;
using DW.ELA.Plugin.Inara.Model;
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
        public void ShouldNotFailOnEvents(LogEvent e)
        {
            var result = eventConverter.Convert(e);
            Assert.NotNull(result);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ApiEvent));
        }
    }
}
