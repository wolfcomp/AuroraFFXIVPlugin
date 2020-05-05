using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AuroraFFXIVPlugin.FFXIV
{
    /// <summary>
    /// Interaction logic for ActionPeeker.xaml
    /// </summary>
    public partial class PlayerPeeker : Window
    {
        public FFXIVMain Ffxiv;

        public PlayerPeeker(FFXIVMain ffxiv)
        {
            Ffxiv = ffxiv;
            ffxiv.MemoryRead += Ffxiv_MemoryRead;
            InitializeComponent();
        }

        private void Ffxiv_MemoryRead()
        {
            JobType.Text = Ffxiv.GameState.Player.JobType.ToString();
            WeaponUnsheathed.Text = Ffxiv.GameState.Player.WeaponUnsheathed.ToString();
            CastingPercentage.Text = Ffxiv.GameState.Player.CastingPercentage.ToString(CultureInfo.InvariantCulture);
            HPCurrent.Text = Ffxiv.GameState.Player.HPCurrent.ToString();
            CPCurrent.Text = Ffxiv.GameState.Player.CPCurrent.ToString();
            MPCurrent.Text = Ffxiv.GameState.Player.MPCurrent.ToString();
            LevelProgression.Text = Ffxiv.GameState.Player.LevelProgression.ToString(CultureInfo.InvariantCulture);
            HPMax.Text = Ffxiv.GameState.Player.HPMax.ToString();
            GPMax.Text = Ffxiv.GameState.Player.GPMax.ToString();
            CPMax.Text = Ffxiv.GameState.Player.CPMax.ToString();
            InCombat.Text = Ffxiv.GameState.Player.InCombat.ToString();
            GPCurrent.Text = Ffxiv.GameState.Player.GPCurrent.ToString();
            MPMax.Text = Ffxiv.GameState.Player.MPMax.ToString();
            Status.Text = Ffxiv.GameState.Player.Status.ToString();
        }
    }
}
