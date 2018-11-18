using EliteLogAgent;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests
{
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
