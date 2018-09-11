using Controller;
using DW.ELA.Utility;
using DW.ELA.Utility.Json;
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
        [Test]
        public void MessagesShouldBeFlushedUponCall()
        {

        }

        /// <summary>
        /// Integration test sending exception record to actual cloud
        /// </summary>
        [Test]
        [Explicit]
        public async Task ErrorsMustBeReportedToCloud()
        {
            var restClient = new ThrottlingRestClient(NLogSettingsManager.CloudErrorReportingUrl);
            var record = new CloudApiLogTarget.ExceptionRecord()
            {
                CallStack = " test at test:123",
                ExceptionType = "TestException",
                Message = "Test exception",
                SoftwareVersion = AppInfo.Version
            };
            await restClient.PostAsync(Serialize.ToJson(record));
        }
    }
}
