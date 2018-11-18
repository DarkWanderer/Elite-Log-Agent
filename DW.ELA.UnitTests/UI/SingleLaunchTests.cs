using EliteLogAgent;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests
{
    public class SingleLaunchTests
    {
        [Test]
        public void AppShouldNotBeLaunched() => Assert.IsFalse(SingleLaunch.IsRunning, "Application may be running");
    }
}
