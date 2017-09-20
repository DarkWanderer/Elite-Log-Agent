using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DW.Inara.LogUploader.Settings;
using Microsoft.Win32;

namespace DW.Inara.LogUploader.Persistence
{
    internal class RegistryInformationStorage : IPersistentFileInfoStorage, IPersistentSettingsStorage
    {
        private static readonly string BaseRegistryKey = Registry.CurrentUser + @"\Software\DarkWanderer\InaraUploader";
        private const string LatestSavedFile = nameof(LatestSavedFile);
        private const string InaraUsername = nameof(InaraUsername);
        private const string InaraPassword = nameof(InaraPassword);

        public string GetLatestSavedFile()
        {
            return Registry.GetValue(BaseRegistryKey, LatestSavedFile, null) as string;
        }

        public UploaderSettings Load()
        {
            var username = Registry.GetValue(BaseRegistryKey, InaraUsername, null) as string;
            var password = Registry.GetValue(BaseRegistryKey, InaraPassword, null) as string;
            if (username != null && password != null)
                return new UploaderSettings { InaraUsername = username, InaraPassword = password };
            else
                return null;
        }

        public void Save(UploaderSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Registry.SetValue(BaseRegistryKey, InaraUsername, settings.InaraUsername, RegistryValueKind.String);
            Registry.SetValue(BaseRegistryKey, InaraPassword, settings.InaraPassword, RegistryValueKind.String);
        }

        public void SetLatestSavedFile(string fileName)
        {
            Registry.SetValue(BaseRegistryKey, LatestSavedFile, fileName, RegistryValueKind.String);
        }
    }
}
