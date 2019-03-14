namespace DW.ELA.UnitTests.UI
{
    using EliteLogAgent.Deployment;
    using NUnit.Framework;

    public class DataPathManagerTests
    {
        [Test]
        public void ShouldProvideDirectories()
        {
            var manager = new DataPathManager();
            Assert.IsNotEmpty(manager.LogDirectory);
            Assert.IsNotEmpty(manager.SettingsDirectory);
        }
    }
}
