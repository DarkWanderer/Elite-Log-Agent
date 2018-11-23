namespace DW.ELA.UnitTests
{
    using EliteLogAgent;
    using NUnit.Framework;

    public class SingleLaunchTests
    {
        [Test]
        public void AppShouldNotBeLaunched() => Assert.IsFalse(SingleLaunch.IsRunning, "Application may be running");
    }
}
