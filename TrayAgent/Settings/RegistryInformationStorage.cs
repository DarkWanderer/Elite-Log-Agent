using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace TrayAgent
{
    internal class RegistryInformationStorage : IPersistentSettingsStorage
    {
        private static readonly string BaseRegistryKey = Registry.CurrentUser + @"\Software\DarkWanderer\EliteLogAgent";
        private const string InaraUsername = nameof(InaraUsername);
        private const string InaraPassword = nameof(InaraPassword);
        static byte[] s_aditionalEntropy = { 1, 9, 8, 8, 0, 8, 2, 8 };

        public UploaderSettings Load()
        {
            var username = Registry.GetValue(BaseRegistryKey, InaraUsername, null) as string;
            var password = Decrypt(Registry.GetValue(BaseRegistryKey, InaraPassword, null) as string);
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
            Registry.SetValue(BaseRegistryKey, InaraPassword, Crypt(settings.InaraPassword), RegistryValueKind.String);
        }

        public static string Crypt(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            try
            {
                var plainBytes = Encoding.Unicode.GetBytes(data);
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                var encryptedData = ProtectedData.Protect(plainBytes, s_aditionalEntropy, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedData);
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        public static string Decrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            try
            {
                var encryptedBytes = Convert.FromBase64String(data);
                //Decrypt the data using DataProtectionScope.CurrentUser.
                var decryptedBytes = ProtectedData.Unprotect(encryptedBytes, s_aditionalEntropy, DataProtectionScope.CurrentUser);
                return Encoding.Unicode.GetString(decryptedBytes);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}