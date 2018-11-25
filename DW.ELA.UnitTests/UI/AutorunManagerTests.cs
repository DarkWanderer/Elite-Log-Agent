namespace DW.ELA.UnitTests
{
    using EliteLogAgent.Autorun;
    using NUnit.Framework;

    public class AutorunManagerTests
    {
        [Test]
        public void ShouldEnableThenDisableAutorun()
        {
            AutorunManager.AutorunEnabled = true;
            Assert.IsTrue(AutorunManager.AutorunEnabled);
            AutorunManager.AutorunEnabled = false;
            Assert.IsFalse(AutorunManager.AutorunEnabled);
        }
    }
}
