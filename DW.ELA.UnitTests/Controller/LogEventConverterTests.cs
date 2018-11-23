namespace DW.ELA.UnitTests
{
    using System;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.LogModel;
    using DW.ELA.UnitTests.Utility;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    public class LogEventConverterTests
    {
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
        [TestCaseSource(typeof(TestEventSource), nameof(TestEventSource.TypedLogEvents))]
        public void EventsTransformationShouldNotSpoilData(LogEvent e)
        {
            if (e.GetType() == typeof(LogEvent))
                Assert.Fail("This test only supports typed events");

            var serialized = JObject.FromObject(e,  Converter.Serializer);

            if (e is Scan)
                e.Raw.Remove("Parents"); // TODO: find a way to serialize that structure

            Assert.IsEmpty(JsonComparer.Compare(e.Event, e.Raw, serialized));

            // This assert should never trigger - if it triggers means there's an error in comparison code
            Assert.IsTrue(JToken.DeepEquals(e.Raw, serialized), "Json objects before/after serialization should be 'DeepEqual'");
        }
    }
}
