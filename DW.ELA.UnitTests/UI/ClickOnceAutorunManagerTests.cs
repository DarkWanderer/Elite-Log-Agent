using EliteLogAgent.Autorun;
using NUnit.Framework;

namespace DW.ELA.UnitTests.UI
{
    public class ClickOnceAutorunManagerTests
    {
        [Test]
        public void ShouldEnableThenDisableClickOnceAutorun()
        {
            var manager = new ClickOnceAutorunManager
            {
                AutorunEnabled = true
            };
            Assert.IsTrue(manager.AutorunEnabled);
            manager.AutorunEnabled = false;
            Assert.IsFalse(manager.AutorunEnabled);
        }

        [Test]
        public void ShouldEnableThenDisablePortableAutorun()
        {
            var manager = new PortableAutorunManager
            {
                AutorunEnabled = true
            };
            Assert.IsTrue(manager.AutorunEnabled);
            manager.AutorunEnabled = false;
            Assert.IsFalse(manager.AutorunEnabled);
        }
    }
}
