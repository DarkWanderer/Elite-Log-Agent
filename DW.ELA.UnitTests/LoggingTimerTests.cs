using DW.ELA.Utility.Log;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests
{
    public class LoggingTimerTests
    {
        [Test]
        public void ShouldShowReasonableTime()
        {
            using (var timer = new LoggingTimer("Test"))
            {
                Assert.That(timer.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
                Assert.That(timer.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));
            }
        }
    }
}
