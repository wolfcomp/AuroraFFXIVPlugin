using Aurora.Settings;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Aurora;
using Application = Aurora.Profiles.Application;

namespace AuroraFFXIVGSIPlugin.FFXIV
{
    /// <summary>
    /// Interaction logic for Control_FFXIV.xaml
    /// </summary>
    public partial class Control_FFXIV : UserControl
    {
        private Application profile_manager;

        public Control_FFXIV(Application profile)
        {
            InitializeComponent();

            profile_manager = profile;

            SetSettings();
        }

        private void SetSettings()
        {
            this.game_enabled.IsChecked = profile_manager.Settings.IsEnabled;
        }

        private void patch_button_Click(object sender, RoutedEventArgs e)
        {
            if (InstallWrapper())
                MessageBox.Show("Aurora Wrapper Patch installed successfully.");
            else
                MessageBox.Show("Aurora Wrapper Patch could not be installed.\r\nGame is not installed.");
        }

        private void unpatch_button_Click(object sender, RoutedEventArgs e)
        {
            if (UninstallWrapper())
                MessageBox.Show("Aurora Wrapper Patch uninstalled successfully.");
            else
                MessageBox.Show("Aurora Wrapper Patch could not be uninstalled.\r\nGame is not installed.");
        }

        private int GameID = 427520;

        private bool InstallWrapper(string installpath = "")
        {
            if (String.IsNullOrWhiteSpace(installpath))
                installpath = Path.Combine(Global.AppDataDirectory, "Plugins");


            if (!String.IsNullOrWhiteSpace(installpath))
            {
                //TODO Get Plugin from github once up there
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool UninstallWrapper()
        {
            String installpath = Path.Combine(Global.AppDataDirectory, "Plugins");
            if (!String.IsNullOrWhiteSpace(installpath))
            {
                string path = System.IO.Path.Combine(Global.AppDataDirectory, "Plugins", "RzChromaSDK.dll");

                if (File.Exists(path))
                    File.Delete(path);

                return true;
            }
            else
            {
                return false;
            }
        }


        private void game_enabled_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                profile_manager.Settings.IsEnabled = (this.game_enabled.IsChecked.HasValue) ? this.game_enabled.IsChecked.Value : false;
                profile_manager.SaveProfiles();
            }
        }
    }
}
