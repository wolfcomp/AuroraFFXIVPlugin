using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora.Profiles;
using Aurora.Settings;
using AuroraFFXIVPlugin.FFXIV.Layers;

namespace AuroraFFXIVPlugin
{
    public class PluginMain : IPlugin
    {
        public string ID { get; private set; } = "AuroraFFXIVPlugin";

        public string Title { get; private set; } = "FFXIV Aurora Plugin";

        public string Author { get; private set; } = "WildWolf";

        public Version Version { get; private set; } = new Version(1, 0, 0);

        private IPluginHost pluginHost;

        public IPluginHost PluginHost
        {
            get { return pluginHost; }
            set { pluginHost = value; }
        }

        private FFXIVMain ffxiv;

        public PluginMain()
        {
            ffxiv = new FFXIVMain();
        }

        public void ProcessManager(object manager)
        {
            if (manager is LightingStateManager stateManager)
            {
                stateManager.RegisterEvent(new FFXIVApplication(ffxiv));
                stateManager.RegisterLayer<FFXIVActionLayerHandler>();
                stateManager.RegisterLayer<FFXIVKeyBindLayerHandler>();
            }
        }
    }
}
