namespace DW.ELA.UnitTests
{
    using System;
    using DW.ELA.Utility.Log;
    using NUnit.Framework;

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
