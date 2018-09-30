using Controller;
using DW.ELA.Plugin.Inara;
using DW.ELA.Plugin.Inara.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DW.ELA.UnitTests.INARA
{
    [TestFixture]
    public class InaraIntegrationTests
    {
        [Test]
        [Explicit]
        public async Task IntegrationTestUploadLatestLogs()
        {
            var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 5);
            var logCounter = new LogEventTypeCounter();
            var stateRecorder = new PlayerStateRecorder();

            var inaraRestClient = new ThrottlingRestClient("https://inara.cz/inapi/v1/");
            var inaraConverter = new InaraEventConverter(stateRecorder);
            var inaraApiFacade = new InaraApiFacade(inaraRestClient, TestCredentials.ApiKey, TestCredentials.UserName);

            var convertedEvents = logEventSource
                .Events
                .SelectMany(inaraConverter.Convert)
                .ToArray();

            var results = await inaraApiFacade.ApiCall(convertedEvents);
        }
    }
}
