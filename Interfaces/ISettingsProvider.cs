using System;
using DW.ELA.Interfaces.Settings;

namespace DW.ELA.Interfaces
{
    public interface ISettingsProvider
    {
        event EventHandler SettingsChanged;

        GlobalSettings Settings { get; set; }
    }
}