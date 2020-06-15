using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Aurora.Profiles;
using Aurora.Settings;
using AuroraFFXIVPlugin.FFXIV;
using AuroraFFXIVPlugin.FFXIV.Layers;

namespace AuroraFFXIVPlugin
{
    public class FFXIVApplication : Application
    {
        public FFXIVMain FFXIVMain;

        public override ImageSource Icon
        {
            get
            {
                BitmapImage b = new BitmapImage();
                b.BeginInit();
                b.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("AuroraFFXIVPlugin.ffxiv_48x48.png");
                b.EndInit();
                return b;
            }
        }

        public FFXIVApplication(FFXIVMain ffxiv) : base(new LightEventConfig
        {
            Name = "FFXIV",
            AppID = "39210",
            ID = "ffxiv",
            Event = new GameEvent_Generic(),
            GameStateType = typeof(FFXIV.GSI.GameState_FFXIV),
            ProfileType = typeof(FFXIVProfile),
            SettingsType = typeof(FirstTimeApplicationSettings),
            OverviewControlType = typeof(Control_FFXIV),
            ProcessNames = new []{ "ffxiv_dx11.exe" }
        })
        {
            FFXIVMain = ffxiv;
            FFXIVMain.MemoryRead += FfxivMain_MemoryRead;
            AllowLayer<FFXIVActionLayerHandler>();
            AllowLayer<FFXIVKeyBindLayerHandler>();
        }

        private void FfxivMain_MemoryRead()
        {
            SetGameState(FFXIVMain.GameState);
        }

        public override void OnStart()
        {
            FFXIVMain.StartReading();
            base.OnStart();
        }

        public override void OnStop()
        {
            FFXIVMain.StopReading();
            base.OnStart();
        }
    }
}
