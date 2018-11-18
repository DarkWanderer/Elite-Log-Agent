using EliteLogAgent.Autorun;
using NUnit.Framework;

namespace DW.ELA.UnitTests
{
    public class AutorunManagerTests
    {
        [Test]
        public void ShouldEnableThenDisableAutorun()
        {
            AutorunManager.AutorunEnabled = false;
            Assert.IsFalse(AutorunManager.AutorunEnabled);
            AutorunManager.AutorunEnabled = true;
            Assert.IsTrue(AutorunManager.AutorunEnabled);
        }
    }
}
