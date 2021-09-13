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
#pragma warning disable CS0618 // Type or member is obsolete
            settings.CommanderName = TestCommander;
            storage.Settings = settings;
            settings = storage.Settings;
            Assert.AreEqual(TestCommander, settings.CommanderName);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
