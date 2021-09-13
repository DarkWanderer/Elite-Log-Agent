using System.Diagnostics;
using System.Linq;
using EliteLogAgent;
using NUnit.Framework;

namespace DW.ELA.UnitTests.UI
{
    public class SingleLaunchTests
    {
        [Test]
        public void AppShouldNotBeLaunched() => Assert.AreEqual(Process.GetProcessesByName("EliteLogAgent").Any(),
                                                                SingleLaunch.IsRunning,
                                                                "SingleLaunch.IsRunning should match actually running process");
    }
}
