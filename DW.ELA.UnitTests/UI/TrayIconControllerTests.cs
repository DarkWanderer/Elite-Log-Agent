using DW.ELA.Interfaces;
using EliteLogAgent;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests.UI
{
    public class TrayIconControllerTests
    {
        [Test]
        public void ShouldBeCreatedWithoutExceptions()
        {
            var pm = Mock.Of<IPluginManager>();
            var sp = Mock.Of<ISettingsProvider>();
            var mb = Mock.Of<IMessageBroker>();

            using (var controller = new TrayIconController(pm, sp, mb)) { };
        }
    }
}
