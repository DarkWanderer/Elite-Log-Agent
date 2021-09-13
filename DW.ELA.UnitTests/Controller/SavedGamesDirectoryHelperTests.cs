using DW.ELA.Controller;
using NUnit.Framework;

namespace DW.ELA.UnitTests.Controller
{
    public class SavedGamesDirectoryHelperTests
    {
        [Test]
        public void ShouldFindSavesDirectory() => Assert.IsNotEmpty(new SavedGamesDirectoryHelper().Directory);
    }
}
