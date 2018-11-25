namespace DW.ELA.Interfaces
{
    using System.Windows.Forms;
    using DW.ELA.Interfaces.Settings;

    public class AbstractSettingsControl : UserControl
    {
        public AbstractSettingsControl()
        {
        }

        /// <summary>
        /// Gets or sets reference to temporary instance of Settings existing in settings form
        /// </summary>
        public GlobalSettings GlobalSettings { get; set; }

        public virtual void SaveSettings() { }
    }
}
