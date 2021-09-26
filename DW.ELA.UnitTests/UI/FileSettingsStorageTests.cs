using DW.ELA.UnitTests.Utility;
using EliteLogAgent;
using NUnit.Framework;

namespace DW.ELA.UnitTests.UI
{
    public class FileSettingsStorageTests
    {
        private const string TestCommander = "TestCommander123";

        [Test]
        public void ShouldSaveLoadSettings()
        {
            var storage = new FileSettingsStorage(new TempDirPathManager());
            var settings = storage.Settings;
        }
    }
}
