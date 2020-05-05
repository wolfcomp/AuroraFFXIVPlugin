using System;
using System.Collections.Generic;
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
    public partial class ActionPeeker : Window
    {
        public FFXIVMain Ffxiv;

        public ActionPeeker(FFXIVMain ffxiv)
        {
            Ffxiv = ffxiv;
            InitializeComponent();
            ffxiv.MemoryRead += Ffxiv_MemoryRead;
        }

        private void Ffxiv_MemoryRead()
        {
            ActionList.ItemsSource = Ffxiv.GameState.Actions.GetList();
        }
    }
}
