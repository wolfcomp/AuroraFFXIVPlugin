using System;
using Aurora.Profiles;
using Aurora.Settings;
using AuroraFFXIVPlugin.FFXIV.Layers;

namespace AuroraFFXIVPlugin
{
    public class PluginMain : IPlugin
    {
        public string ID => "AuroraFFXIVPlugin";

        public string Title => "FFXIV Aurora Plugin";

        public string Author => "WildWolf";

        public Version Version => new Version(1, 0, 0);

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
