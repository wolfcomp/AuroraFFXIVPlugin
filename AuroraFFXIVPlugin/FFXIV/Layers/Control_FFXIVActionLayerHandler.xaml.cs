using Aurora.Settings;
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
using Aurora.Utils;
using Xceed.Wpf.Toolkit;
using Application = Aurora.Profiles.Application;

namespace AuroraFFXIVPlugin.FFXIV.Layers
{
    /// <summary>
    /// Interaction logic for Control_FFXIVActionLayerHandler.xaml
    /// </summary>
    public partial class Control_FFXIVActionLayerHandler : UserControl
    {
        private bool settingsset = false;

        public Control_FFXIVActionLayerHandler()
        {
            InitializeComponent();
        }

        public Control_FFXIVActionLayerHandler(FFXIVActionLayerHandler datacontext)
        {
            InitializeComponent();

            this.DataContext = datacontext;
        }

        public void SetSettings()
        {
            if (this.DataContext is FFXIVActionLayerHandler dataContext && !settingsset)
            {
                this.ColorPicker_Primary.SelectedColor = ColorUtils.DrawingColorToMediaColor(dataContext.Properties._PrimaryColor ?? System.Drawing.Color.Empty);
                this.ColorPicker_Combo.SelectedColor = ColorUtils.DrawingColorToMediaColor(dataContext.Properties._Combo ?? System.Drawing.Color.Empty);
                this.ColorPicker_Range.SelectedColor = ColorUtils.DrawingColorToMediaColor(dataContext.Properties._NotAvailable ?? System.Drawing.Color.Empty);
                this.ColorPicker_Available.SelectedColor = ColorUtils.DrawingColorToMediaColor(dataContext.Properties._OutOfRange ?? System.Drawing.Color.Empty);

                settingsset = true;
            }
        }

        internal void SetProfile(Application profile)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetSettings();

            this.Loaded -= UserControl_Loaded;
        }

        private void ColorPicker_Primary_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (IsLoaded && settingsset && this.DataContext is FFXIVActionLayerHandler dataContext && sender is Xceed.Wpf.Toolkit.ColorPicker && (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColor.HasValue)
                dataContext.Properties._PrimaryColor = ColorUtils.MediaColorToDrawingColor((sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColor.Value);
        }

        private void ColorPicker_Combo_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (IsLoaded && settingsset && this.DataContext is FFXIVActionLayerHandler dataContext && sender is Xceed.Wpf.Toolkit.ColorPicker && (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColor.HasValue)
                dataContext.Properties._Combo = ColorUtils.MediaColorToDrawingColor((sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColor.Value);
        }

        private void ColorPicker_Range_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (IsLoaded && settingsset && this.DataContext is FFXIVActionLayerHandler dataContext && sender is Xceed.Wpf.Toolkit.ColorPicker && (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColor.HasValue)
                dataContext.Properties._OutOfRange = ColorUtils.MediaColorToDrawingColor((sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColor.Value);
        }

        private void ColorPicker_Available_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (IsLoaded && settingsset && this.DataContext is FFXIVActionLayerHandler dataContext && sender is Xceed.Wpf.Toolkit.ColorPicker && (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColor.HasValue)
                dataContext.Properties._NotAvailable = ColorUtils.MediaColorToDrawingColor((sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColor.Value);
        }
    }
}
