using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurora;
using Aurora.Profiles;
using Aurora.Profiles.EliteDangerous;
using Aurora.Settings;
using AuroraFFXIVGSIPlugin.FFXIV;
using AuroraFFXIVGSIPlugin.FFXIV.GSI;
using AuroraFFXIVGSIPlugin.FFXIV.Layers;
using Newtonsoft.Json;

namespace AuroraFFXIVGSIPlugin
{
    public class FFXIVApplication : Application
    {
        private FFXIVMain ffxivMain;

        public FFXIVApplication(FFXIVMain ffxiv) : base(new LightEventConfig
        {
            Name = "FFXIV",
            AppID = "39210",
            ID = "ffxiv",
            Event = new GameEvent_Generic(),
            IconURI = "Resources/ffxiv_48x48.png",
            GameStateType = typeof(FFXIV.GSI.GameState_FFXIV),
            ProfileType = typeof(FFXIVProfile),
            SettingsType = typeof(FirstTimeApplicationSettings),
            OverviewControlType = typeof(Control_FFXIV),
            ProcessNames = new []{ "ffxiv_dx11.exe" }
        })
        {
            ffxivMain = ffxiv;
            ffxivMain.MemoryRead += FfxivMain_MemoryRead;
            var extra = new List<LayerHandlerEntry>
            {
                new LayerHandlerEntry("FFXIVActionLayer", "FFXIV Actions Layer", typeof(FFXIVActionLayerHandler)),
                new LayerHandlerEntry("FFXIVKeyBindLayer", "FFXIV Key Binds Layer", typeof(FFXIVKeyBindLayerHandler))
            };

            Global.LightingStateManager.RegisterLayerHandlers(extra, false);

            foreach (var entry in extra)
            {
                Config.ExtraAvailableLayers.Add(entry.Key);
            }
        }

        private void FfxivMain_MemoryRead()
        {
            SetGameState(ffxivMain.GameState);
        }

        public override void OnStart()
        {
            ffxivMain.StartReading();
            base.OnStart();
        }

        public override void OnStop()
        {
            ffxivMain.StopReading();
            base.OnStart();
        }
    }
}
