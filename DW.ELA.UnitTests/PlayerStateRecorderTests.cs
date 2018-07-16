using Controller;
using DW.ELA.Interfaces;
using NUnit.Framework;
using System;

namespace DW.ELA.UnitTests
{
    [TestFixture]
    public class PlayerStateRecorderTests
    {

        private readonly IPlayerStateHistoryRecorder eventConverter = new PlayerStateRecorder();

        [Test]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.LogEvents))]
        public void ShouldNotFailOnEvents(LogEvent e)
        {
            eventConverter.OnNext(e);

            eventConverter.GetPlayerSystem(DateTime.UtcNow);
            eventConverter.GetPlayerShipType(DateTime.UtcNow);
            eventConverter.GetPlayerShipId(DateTime.UtcNow);
            eventConverter.GetPlayerCrewStatus(DateTime.UtcNow);
        }
    }
}