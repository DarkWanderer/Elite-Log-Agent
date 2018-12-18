namespace DW.ELA.UnitTests
{
    using EliteLogAgent.Autorun;
    using NUnit.Framework;

    public class ClickOnceAutorunManagerTests
    {
        [Test]
        public void ShouldEnableThenDisableClickOnceAutorun()
        {
            var manager = new ClickOnceAutorunManager();
            manager.AutorunEnabled = true;
            Assert.IsTrue(manager.AutorunEnabled);
            manager.AutorunEnabled = false;
            Assert.IsFalse(manager.AutorunEnabled);
        }

        [Test]
        public void ShouldEnableThenDisablePortableAutorun()
        {
            var manager = new PortableAutorunManager();
            manager.AutorunEnabled = true;
            Assert.IsTrue(manager.AutorunEnabled);
            manager.AutorunEnabled = false;
            Assert.IsFalse(manager.AutorunEnabled);
        }

    }
}
