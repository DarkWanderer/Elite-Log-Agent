namespace DW.ELA.UnitTests.Controller
{
    using DW.ELA.Controller;
    using NUnit.Framework;

    public class SavedGamesDirectoryHelperTests
    {
        [Test]
        public void ShouldFindSavesDirectory() => Assert.IsNotEmpty(new SavedGamesDirectoryHelper().Directory);
    }
}
