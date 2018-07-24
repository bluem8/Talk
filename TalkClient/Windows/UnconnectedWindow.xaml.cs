using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace TalkClient.Windows
{
    /// <summary>
    /// Interaction logic for UnconnectedWindow.xaml
    /// </summary>
    public partial class UnconnectedWindow : Window
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public UnconnectedWindow()
        {
            AllocConsole();
            InitializeComponent();
            this.WindowStyle = WindowStyle.ToolWindow;
            this.ResizeMode = ResizeMode.CanMinimize;
            UsernameTextbox.Text = Environment.MachineName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = new MainWindow(AddressTextbox.Text, Int32.Parse(PortTextbox.Text), UsernameTextbox.Text);
            m.Show();
            m.Closed += M_Closed;
            this.Hide();
            
        }

        private void M_Closed(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}
