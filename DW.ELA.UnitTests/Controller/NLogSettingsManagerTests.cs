namespace DW.ELA.UnitTests.Controller
{
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.UnitTests.Utility;
    using Moq;
    using NLog;
    using NUnit.Framework;

    public class NLogSettingsManagerTests
    {
        [Test]
        public void ShouldSetupLogByDefault()
        {
            var manager = new NLogSettingsManager(new Mock<ISettingsProvider>().Object, new TempDirPathManager());
            manager.Setup();

            // Reset log
            LogManager.Configuration = new NLog.Config.LoggingConfiguration();
        }
    }
}
