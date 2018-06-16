using DW.ELA.Interfaces.Settings;
using System;

namespace Interfaces
{
    public interface ISettingsProvider
    {
        GlobalSettings Settings { get; set; }
        event EventHandler SettingsChanged;
    }
}