using Controller;
using DW.ELA.UnitTests.INARA;
using DW.ELA.Utility;
using DW.ELA.Utility.Crypto;
using DW.ELA.Utility.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DW.ELA.UnitTests
{
    public class CloudApiLogTargetTests
    {
        private Exception CreateTestException()
        {
            try
            {
                try
                {
                    var innerException = new ArgumentException("Test 1");
                    innerException.Data["test"] = "testdata";
                    throw innerException;
                }
                catch (Exception e1)
                {
                    throw new ApplicationException("Test 2", e1);
                }
            }
            catch (Exception e2)
            {
                return e2;
            }
            throw new ApplicationException("Something went wrong");
        }

        [Test]
        public void MessagesShouldBeFlushedUponCall()
        {

        }

        /// <summary>
        /// Integration test sending exception record to actual cloud
        /// </summary>
        [Test]
        [Explicit]
        public async Task IntegrationTestCloudApiErrorReporting()
        {
            var restClient = new ThrottlingRestClient(NLogSettingsManager.CloudErrorReportingUrl);
            string serialized = Serialize.ToJson(CreateTestException());
            string cmdrHash = Hash.Sha1(TestCredentials.UserName);

            var record = new CloudApiLogTarget.ExceptionRecord(AppInfo.Version, cmdrHash, serialized);
            var array = new JArray(JObject.FromObject(record));
            await restClient.PostAsync(array.ToString());
        }

        [Test]
        public void ExceptionShouldBeSerialized()
        {
            string serialized = Serialize.ToJson(CreateTestException());
            Assert.Pass(serialized);
        }
    }
}
