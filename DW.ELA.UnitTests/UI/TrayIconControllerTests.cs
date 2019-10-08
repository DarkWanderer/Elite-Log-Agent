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
            var ps = Mock.Of<IPlayerStateHistoryRecorder>();
            var am = Mock.Of<IAutorunManager>();
            var dm = Mock.Of<IPathManager>();

            using (var controller = new TrayIconController(pm, sp, ps, am, dm))
            {
            }
        }
    }
}
