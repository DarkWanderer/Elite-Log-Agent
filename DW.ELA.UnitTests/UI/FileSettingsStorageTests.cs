namespace DW.ELA.UnitTests
{
    using System.IO;
    using EliteLogAgent;
    using NUnit.Framework;

    public class FileSettingsStorageTests
    {
        private const string TestCommander = "TestCommander123";

        [Test]
        public void ShouldSaveLoadSettings()
        {
            var storage = new FileSettingsStorage() { SettingsFileDirectory = Path.GetTempPath() };
            var settings = storage.Settings;
            settings.CommanderName = TestCommander;
            storage.Settings = settings;
            settings = storage.Settings;
            Assert.AreEqual(TestCommander, settings.CommanderName);
        }
    }
}
