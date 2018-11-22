namespace DW.ELA.UnitTests.INARA
{
    using System.Linq;
    using System.Threading.Tasks;
    using DW.ELA.Controller;
    using DW.ELA.Plugin.Inara;
    using DW.ELA.Plugin.Inara.Model;
    using DW.ELA.Utility;
    using NUnit.Framework;

    [TestFixture]
    public class InaraIntegrationTests
    {
        [Test]
        [Explicit]
        public async Task IntegrationTestUploadToInara()
        {
            var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 5);
            var logCounter = new LogEventTypeCounter();
            var stateRecorder = new PlayerStateRecorder();

            var inaraRestClient = new ThrottlingRestClient("https://inara.cz/inapi/v1/");
            var inaraConverter = new InaraEventConverter(stateRecorder);
            var inaraApiFacade = new InaraApiFacade(inaraRestClient, TestCredentials.ApiKey, TestCredentials.UserName);

            // Populate the state recorder to avoid missing ships/starsystems data
            foreach (var ev in logEventSource.Events)
                stateRecorder.OnNext(ev);

            var convertedEvents = logEventSource
                .Events
                .SelectMany(inaraConverter.Convert)
                .ToArray();

            var results = await inaraApiFacade.ApiCall(convertedEvents);
            results = results
                .Where(e => e.EventStatus != 200)
                .Where(e => e.EventStatusText != "There is a newer inventory state recorded already.")
                .Where(e => e.EventStatusText != "This ship was not found but was added automatically.")
                .ToList();

            CollectionAssert.IsEmpty(results);
            Assert.Pass("Uploaded {0} events", convertedEvents.Length);
        }
    }
}
