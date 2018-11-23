namespace DW.ELA.UnitTests
{
    using System;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using NUnit.Framework;

    [TestFixture]
    public class PlayerStateRecorderTests
    {
        private readonly IPlayerStateHistoryRecorder eventConverter = new PlayerStateRecorder();

        [Test]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void ShouldNotFailOnEvents(LogEvent e)
        {
            eventConverter.OnNext(e);

            eventConverter.GetPlayerSystem(DateTime.UtcNow);
            eventConverter.GetPlayerShipType(DateTime.UtcNow);
            eventConverter.GetPlayerShipId(DateTime.UtcNow);
            eventConverter.GetPlayerIsInCrew(DateTime.UtcNow);
        }

        private const string ValidShip = "Valid_Ship";
        private const long ValidShipId = 77;

        [Test]
        public void ShouldNotRecordFighterOrSrv()
        {
            var time1 = DateTime.UtcNow;
            var time2 = time1.AddSeconds(5);
            var time3 = time2.AddSeconds(5);
            var time4 = time3.AddSeconds(5);

            eventConverter.OnNext(new LoadGame() { Timestamp = time1, Ship = ValidShip, ShipId = ValidShipId });
            Assert.AreEqual(ValidShip, eventConverter.GetPlayerShipType(time4));
            Assert.AreEqual(ValidShipId, eventConverter.GetPlayerShipId(time4));

            eventConverter.OnNext(new LoadGame() { Timestamp = time2, Ship = "Some_Fighter", ShipId = 567 });
            Assert.AreEqual(ValidShip, eventConverter.GetPlayerShipType(time4));
            Assert.AreEqual(ValidShipId, eventConverter.GetPlayerShipId(time4));

            eventConverter.OnNext(new LoadGame() { Timestamp = time3, Ship = "testbuggy", ShipId = 123 });
            Assert.AreEqual(ValidShip, eventConverter.GetPlayerShipType(time4));
            Assert.AreEqual(ValidShipId, eventConverter.GetPlayerShipId(time4));
        }
    }
}
