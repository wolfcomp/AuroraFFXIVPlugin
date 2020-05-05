using Aurora.Settings;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Aurora;
using Application = Aurora.Profiles.Application;

namespace AuroraFFXIVPlugin.FFXIV
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

        private void actions_button_Click(object sender, RoutedEventArgs e)
        {
            if (profile_manager is FFXIVApplication ffxivApplication)
                new ActionPeeker(ffxivApplication.FFXIVMain).Show();
        }

        private void player_button_Click(object sender, RoutedEventArgs e)
        {
            if (profile_manager is FFXIVApplication ffxivApplication)
                new PlayerPeeker(ffxivApplication.FFXIVMain).Show();
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
