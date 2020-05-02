using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora.Settings;

namespace AuroraFFXIVGSIPlugin
{
    public class PluginMain : IPlugin
    {
        public string ID { get; private set; } = "AuroraFFXIVGSIPlugin";

        public string Title { get; private set; } = "FFXIV Aurora GSI Plugin";

        public string Author { get; private set; } = "WildWolf";

        public Version Version { get; private set; } = new Version(0, 1);

        private IPluginHost pluginHost;

        public IPluginHost PluginHost
        {
            get { return pluginHost; }
            set { pluginHost = value; }
        }

        private Main main;

        public PluginMain()
        {
            main = new Main();
            main.MainAsync(new string[] { });
        }

        public void ProcessManager(object manager) { }
    }
}
