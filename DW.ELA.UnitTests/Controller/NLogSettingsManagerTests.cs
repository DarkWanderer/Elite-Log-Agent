namespace DW.ELA.UnitTests.Controller
{
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using Moq;
    using NLog;
    using NUnit.Framework;

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
