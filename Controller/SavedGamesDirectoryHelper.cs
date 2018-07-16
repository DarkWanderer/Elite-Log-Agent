using DW.ELA.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace Controller
{
    public class SavedGamesDirectoryHelper : ILogDirectoryNameProvider
    {
        public string Directory => SDirectory;

        private static string SDirectory
        {
            get
            {
                int result = UnsafeNativeMethods.SHGetKnownFolderPath(new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"), 0, new IntPtr(0), out IntPtr path);
                if (result >= 0)
                {
                    return Marshal.PtrToStringUni(path) + @"\Frontier Developments\Elite Dangerous";
                }
                else
                {
                    throw new ExternalException("Failed to find the saved games directory.", result);
                }
            }
        }

        private static class UnsafeNativeMethods
        {
            [DllImport("Shell32.dll")]
            public static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);
        }
    }
}
