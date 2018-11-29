namespace DW.ELA.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.LogModel;
    using DW.ELA.UnitTests.Utility;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable]
    public class LogEventConverterTests
    {
        private static IEnumerable<TestCaseData> RawTestCases => TestEventSource.CannedEventsRaw
            .Select(jo => new TestCaseData(jo).SetArgDisplayNames(jo.Property("event").Value.ToString()));

        [Test]
        public void ShouldConvertFsdJumpEvent()
        {
            string eventString = @"{""timestamp"":""2018-06-25T18:10:30Z"", ""event"":""FSDJump"", ""StarSystem"":""Shinrarta Dezhra"", 
                ""SystemAddress"":3932277478106, ""StarPos"":[55.71875, 17.59375, 27.15625 ], ""SystemAllegiance"":""PilotsFederation"", 
                ""SystemEconomy"":""$economy_HighTech;"", ""SystemEconomy_Localised"":""High Tech"", ""SystemSecondEconomy"":""$economy_Industrial;"", 
                ""SystemSecondEconomy_Localised"":""Industrial"", ""SystemGovernment"":""$government_Democracy;"", ""SystemGovernment_Localised"":""Democracy"", 
                ""SystemSecurity"":""$SYSTEM_SECURITY_high;"", ""SystemSecurity_Localised"":""High Security"", ""Population"":85206935, ""JumpDist"":11.896, 
                ""FuelUsed"":2.983697, ""FuelLevel"":12.767566, ""Factions"":[{""Name"":""Lori Jameson"", ""FactionState"":""None"", ""Government"":""Engineer"", 
                ""Influence"":0.000000, ""Allegiance"":""Independent""} ], ""SystemFaction"":""Pilots Federation Local Branch""}";

            var @event = (FsdJump)LogEventConverter.Convert(JObject.Parse(eventString));
            Assert.AreEqual(new DateTime(2018, 06, 25, 18, 10, 30, DateTimeKind.Utc), @event.Timestamp);
        }

        [Test]
        [Parallelizable]
        [TestCaseSource(typeof(LogEventConverterTests), nameof(RawTestCases))]
        public void EventsTransformationShouldNotSpoilData(JObject source)
        {
            var @event = LogEventConverter.Convert(source);

            if (@event.GetType() == typeof(LogEvent))
                Assert.Pass("Automatic pass for non-typed events");

            var serialized = JObject.FromObject(@event, Converter.Serializer);

            if (@event is Scan)
                @event.Raw.Remove("Parents"); // TODO: find a way to serialize that structure

            Assert.IsEmpty(JsonComparer.Compare(@event.Event, @event.Raw, serialized));

            // This assert should never trigger - if it triggers means there's an error in comparison code
            Assert.IsTrue(JToken.DeepEquals(@event.Raw, serialized), "Json objects before/after serialization should be 'DeepEqual'");
        }

        [Test]
        [Explicit]
        public void ShouldConvertLocalEvents()
        {
            var events = TestEventSource.LocalBetaEvents.Concat(TestEventSource.LocalEvents).ToList();
            events.ForEach(x => LogEventConverter.Convert(x));
        }
    }
}
