using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests.ELA
{
    public static class ContainerBootstrapperTests
    {
        [Test]
        public static void ShouldConfigureContainer()
        {
            using (var container = new Windsor)
        }
    }
}
