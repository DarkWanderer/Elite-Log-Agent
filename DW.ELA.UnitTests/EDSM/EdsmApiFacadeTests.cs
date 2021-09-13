using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DW.ELA.Interfaces;
using DW.ELA.Plugin.EDSM;
using DW.ELA.UnitTests.Utility;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DW.ELA.UnitTests.EDSM
{
    public class EdsmApiFacadeTests
    {
        [Test]
        public void ShouldThrowExceptionIfMoreThan100Events()
        {
            var restClientMock = new Mock<IRestClient>(MockBehavior.Strict);
            var facade = new EdsmApiFacade(restClientMock.Object, TestCredentials.UserName, TestCredentials.Edsm.ApiKey);

            var events = Enumerable.Range(1, 200).Select(i => new JObject()).ToArray();
            Assert.ThrowsAsync<ArgumentException>(() => facade.PostLogEvents(events));

            restClientMock.VerifyAll();
        }

        [Test]
        public void ShouldNotSendZeroEvents()
        {
            var restClientMock = new Mock<IRestClient>(MockBehavior.Strict);
            var facade = new EdsmApiFacade(restClientMock.Object, TestCredentials.UserName, TestCredentials.Edsm.ApiKey);

            Assert.DoesNotThrowAsync(() => facade.PostLogEvents(Array.Empty<JObject>()));

            restClientMock.VerifyAll();
        }

        [Test]
        public void ShouldSendEvents()
        {
            var restClientMock = new Mock<IRestClient>();
            restClientMock.Setup(c => c.PostAsync(It.IsAny<IDictionary<string, string>>())).Returns(Task.FromResult("{ \"msgnum\": 100}")).Verifiable();

            var facade = new EdsmApiFacade(restClientMock.Object, TestCredentials.UserName, TestCredentials.Edsm.ApiKey);

            var events = Enumerable.Range(1, 5).Select(i => new JObject()).ToArray();
            Assert.DoesNotThrowAsync(() => facade.PostLogEvents(events));

            restClientMock.Verify(c => c.PostAsync(It.IsAny<IDictionary<string, string>>()), Times.Once());
            restClientMock.Verify(c => c.PostAsync(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void ShouldThrowOnError()
        {
            var restClientMock = new Mock<IRestClient>();
            restClientMock.Setup(c => c.PostAsync(It.IsAny<IDictionary<string, string>>())).Returns(Task.FromResult("{ \"msgnum\": 100}")).Verifiable();

            var facade = new EdsmApiFacade(restClientMock.Object, TestCredentials.UserName, TestCredentials.Edsm.ApiKey);

            var events = Enumerable.Range(1, 5).Select(i => new JObject()).ToArray();
            Assert.DoesNotThrowAsync(() => facade.PostLogEvents(events));

            restClientMock.Verify(c => c.PostAsync(It.IsAny<IDictionary<string, string>>()), Times.Once());
            restClientMock.Verify(c => c.PostAsync(It.IsAny<string>()), Times.Never());
        }
    }
}
