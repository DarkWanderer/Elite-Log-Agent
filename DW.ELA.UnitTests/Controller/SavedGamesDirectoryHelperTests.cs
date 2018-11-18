using Controller;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests
{
    public class SavedGamesDirectoryHelperTests
    {
        [Test]
        public void ShouldFindSavesDirectory() => Assert.IsNotEmpty(new SavedGamesDirectoryHelper().Directory);
    }
}
