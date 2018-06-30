using DW.ELA.Interfaces;
using DW.ELA.LogModel.Events;
using DW.ELA.Plugin.EDDN;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests
{
    [TestFixture]
    public class EddnEventConverterTests
    {
        private readonly EddnEventConverter eventConverter = new EddnEventConverter();

        [Test]
        [TestCaseSource(typeof(TestEventSource),nameof(TestEventSource.LogEvents))]
        public void ShouldNotFailOnEvents(LogEvent e) => eventConverter.Convert(e);
    }
}
