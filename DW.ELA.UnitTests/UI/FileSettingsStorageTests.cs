namespace DW.ELA.UnitTests.UI
{
    using System.IO;
    using DW.ELA.Interfaces;
    using DW.ELA.UnitTests.Utility;
    using EliteLogAgent;
    using NUnit.Framework;

    public class FileSettingsStorageTests
    {
        private const string TestCommander = "TestCommander123";

        [Test]
        public void ShouldSaveLoadSettings()
        {
            var storage = new FileSettingsStorage(new TempDirPathManager());
            var settings = storage.Settings;
            settings.CommanderName = TestCommander;
            storage.Settings = settings;
            settings = storage.Settings;
            Assert.AreEqual(TestCommander, settings.CommanderName);
        }
    }
}
