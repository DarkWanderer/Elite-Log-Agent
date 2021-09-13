using System.Reflection;

namespace EliteLogAgent.Autorun
{
    public class PortableAutorunManager : ClickOnceAutorunManager
    {
        protected override string ExecutablePath => Assembly.GetExecutingAssembly().Location;
    }
}
