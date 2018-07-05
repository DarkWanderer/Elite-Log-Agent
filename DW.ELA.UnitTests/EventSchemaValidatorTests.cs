using DW.ELA.Plugin.EDDN;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests
{
    public class EventSchemaValidatorTests
    {
        [Test]
        public void ShouldNotValidateEmptyObject()
        {
            var @event = new JournalEvent();
            Assert.IsFalse(new EventSchemaValidator().ValidateSchema(@event));
        }

        [Test]
        public void ShouldNotValidateBadSchema()
        {
            var @event = new TestEventType();
            Assert.IsFalse(new EventSchemaValidator().ValidateSchema(@event));
        }

        private class TestEventType : EddnEvent
        {
            public override string SchemaRef => "unknown schema";
        }
    }
}
