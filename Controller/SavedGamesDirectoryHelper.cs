namespace DW.ELA.Controller
{
    using System;
    using System.Runtime.InteropServices;
    using DW.ELA.Interfaces;

    public class SavedGamesDirectoryHelper : ILogDirectoryNameProvider
    {
        private readonly Lazy<string> lazyDirectoryValue = new Lazy<string>(GetSavedGamesDirectory);

        public string Directory => lazyDirectoryValue.Value;

        private static string GetSavedGamesDirectory()
        {
            int result = UnsafeNativeMethods.SHGetKnownFolderPath(new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"), 0, new IntPtr(0), out var path);
            if (result >= 0)
            {
                try
                {
                    return Marshal.PtrToStringUni(path) + @"\Frontier Developments\Elite Dangerous";
                }
                finally { Marshal.FreeCoTaskMem(path); }
            }
            else
            {
                throw new ExternalException("Failed to find the saved games directory.", result);
            }
        }

        private static class UnsafeNativeMethods
        {
            [DllImport("Shell32.dll")]
            public static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);
        }
    }
}
