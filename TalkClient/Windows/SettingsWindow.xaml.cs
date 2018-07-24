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
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TalkClient.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.ToolWindow;
            ImageXText.Text = Properties.Settings.Default.ScreenshotDownsampleSizeX.ToString();
            ImageYText.Text = Properties.Settings.Default.ScreenshotDownsampleSizeY.ToString();

            foreach (InterpolationMode e in (InterpolationMode[])Enum.GetValues(typeof(InterpolationMode)))
            {
                InterpModeList.Items.Add(e.ToString());
            }

            InterpModeList.SelectedIndex = (int)Properties.Settings.Default.ScreenshotInterpolationMode;

            foreach (CompositingQuality e in (CompositingQuality[])Enum.GetValues(typeof(CompositingQuality)))
            {
                CompList.Items.Add(e.ToString());
            }

            CompList.SelectedIndex = (int)Properties.Settings.Default.ScreenshotCompositingQuality;

            foreach (Keys e in (Keys[])Enum.GetValues(typeof(Keys)))
            {
                PTTList.Items.Add(e);
            }
            PTTList.SelectedIndex = (int)Keys.A;

            if (Properties.Settings.Default.ScreenshotDownsample)
            {
                KeepSizeRadio.IsChecked = true;
                ResizeRadio.IsChecked = false;
            }
            else
            {
                KeepSizeRadio.IsChecked = false;
                ResizeRadio.IsChecked = true;
            }

            if (Properties.Settings.Default.UsePTT)
            {
                PPTButton.IsChecked = true;
            }
            else
            {
                PPTButton.IsChecked = false;
            }

        }

        void TabControl_SelectionChanged(object s, EventArgs e)
        {

        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            int y = 0;

            if(Int32.TryParse(ImageXText.Text,out x) && Int32.TryParse(ImageYText.Text, out y)){

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid size");
            }
        }

        private void InterpModeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ApplyButton_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ScreenshotDownsampleSizeX = int.Parse(ImageXText.Text);
            Properties.Settings.Default.ScreenshotDownsampleSizeY = int.Parse(ImageYText.Text);
            Properties.Settings.Default.ScreenshotCompositingQuality = (int)CompList.SelectedIndex;
            Properties.Settings.Default.ScreenshotInterpolationMode = (int)InterpModeList.SelectedIndex;

            Properties.Settings.Default.PTTKey = (Keys)((int)PTTList.SelectedItem);
            Properties.Settings.Default.UsePTT = (bool)PPTButton.IsChecked;
        }
    }
}
