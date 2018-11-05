using Controller;
using DW.ELA.Interfaces;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests.Controller
{
    public class NLogSettingsManagerTests
    {
        [Test]
        public void ShouldSetupLogByDefault()
        {
            var manager = new NLogSettingsManager(new Mock<ISettingsProvider>().Object);
            manager.Setup();
            // Reset log
            LogManager.Configuration = new NLog.Config.LoggingConfiguration();
        }
    }
}
