using EliteLogAgent.Autorun;
using NUnit.Framework;

namespace DW.ELA.UnitTests
{
    class AutorunManagerTests
    {
        [Test]
        void ShouldEnableThenDisableAutorun()
        {
            Assert.IsFalse(AutorunManager.AutorunEnabled);
            AutorunManager.AutorunEnabled = true;
            Assert.IsTrue(AutorunManager.AutorunEnabled);
            AutorunManager.AutorunEnabled = false;
            Assert.IsFalse(AutorunManager.AutorunEnabled);
        }
    }
}
