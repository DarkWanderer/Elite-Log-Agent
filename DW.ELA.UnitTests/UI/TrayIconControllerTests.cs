namespace DW.ELA.UnitTests.UI
{
    using DW.ELA.Interfaces;
    using EliteLogAgent;
    using Moq;
    using NUnit.Framework;

    public class TrayIconControllerTests
    {
        [Test]
        public void ShouldBeCreatedWithoutExceptions()
        {
            var pm = Mock.Of<IPluginManager>();
            var sp = Mock.Of<ISettingsProvider>();
            var mb = Mock.Of<IMessageBroker>();
            var am = Mock.Of<IAutorunManager>();

            using (var controller = new TrayIconController(pm, sp, mb, am))
            {
            }
        }
    }
}
