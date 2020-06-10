namespace DW.ELA.UnitTests.Controller
{
    using System;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class PlayerStateRecorderTests
    {
        private readonly IPlayerStateHistoryRecorder eventConverter = new PlayerStateRecorder();
        private const string ValidShip = "Valid_Ship";
        private const long ValidShipId = 77;

        [Test]
        [Parallelizable]
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.CannedEvents))]
        public void ShouldNotFailOnEvents(JournalEvent e)
        {
            eventConverter.OnNext(e);

            eventConverter.GetPlayerSystem(DateTime.UtcNow);
            eventConverter.GetPlayerShipType(DateTime.UtcNow);
            eventConverter.GetPlayerShipId(DateTime.UtcNow);
            eventConverter.GetPlayerIsInCrew(DateTime.UtcNow);
        }

        [Test]
        public void ShouldNotRecordFighterOrSrv()
        {
            var time1 = DateTime.UtcNow;
            var time2 = time1.AddSeconds(5);
            var time3 = time2.AddSeconds(5);
            var time4 = time3.AddSeconds(5);

            var e1 = new LoadGame() { Timestamp = time1, Ship = ValidShip, ShipId = ValidShipId };
            var e2 = new LoadGame() { Timestamp = time2, Ship = "Some_Fighter", ShipId = 567 };
            var e3 = new LoadGame() { Timestamp = time3, Ship = "testbuggy", ShipId = 123 };
            foreach (var e in new[] { e1, e2, e3 })
            {
                e.Raw = JObject.FromObject(e);
                e.Event = e.GetType().Name;
            }


            eventConverter.OnNext(e1);
            Assert.AreEqual(ValidShip, eventConverter.GetPlayerShipType(time4));
            Assert.AreEqual(ValidShipId, eventConverter.GetPlayerShipId(time4));

            eventConverter.OnNext(e2);
            Assert.AreEqual(ValidShip, eventConverter.GetPlayerShipType(time4));
            Assert.AreEqual(ValidShipId, eventConverter.GetPlayerShipId(time4));

            eventConverter.OnNext(e3);
            Assert.AreEqual(ValidShip, eventConverter.GetPlayerShipType(time4));
            Assert.AreEqual(ValidShipId, eventConverter.GetPlayerShipId(time4));
        }
    }
}
