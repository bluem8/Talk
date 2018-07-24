using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TalkLib;

using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;

namespace TalkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServerConnection _server;
        Guid ThisId;
        Packet_ChannelList ChannelList;
        NotifyIcon Notifications;
        Bitmap LatestImage = null;
        AudioManager AudioInput;
        List<Packet_ImageFromClient> ReceivedImagesList = new List<Packet_ImageFromClient>();

        public MainWindow(string addr, int port, string username)
        {
            HotKeyManager.hello();

            AudioInput = new AudioManager();
            AudioInput.DataReceieved += AudioInput_DataReceieved;

            
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            Notifications = new NotifyIcon();
            Notifications.Icon = TalkClient.Properties.Resources.favicon;
            Notifications.BalloonTipClicked += Notifications_BalloonTipClicked;
            BuildContextMenu();

            Notifications.Visible = true;
            this.Closing += MainWindow_Closing;
            ThisId = Guid.NewGuid();
            this.Title = ThisId.ToString();
            this.ShowInTaskbar = true;
            DisconnectButton.Visibility = Visibility.Hidden;
            ConsoleText.IsReadOnly = true;
            ConsoleText.Background = System.Windows.Media.Brushes.Black;
            ConsoleText.Foreground = System.Windows.Media.Brushes.Green;
            ConsoleText.Document.Blocks.Clear();
            ConsoleText.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            ClientListBox.PreviewMouseDoubleClick += ClientListBox_PreviewMouseDoubleClick;
            ClientListBox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            ClientListBox.Background = System.Windows.Media.Brushes.LightGray;
            _server = new ServerConnection();
            _server.Connected += Event_Connected;
            _server.ConnectionFailed += Event_ConnectionFailed;
            _server.DataReceived += _server_DataReceived;
            _server.Disconnected += _server_Disconnected;
            _server.ServerShutdown += _server_ServerShutdown;
            _server.BpsUpdated += _server_BpsUpdated;
            _server.Connect(new System.Net.IPEndPoint(IPAddress.Parse(addr), port), ThisId, username);
        }

        private void _server_BpsUpdated(object sender, object[] e)
        {
            long sent = (long)e[0];
            long received = (long)e[1];

            this.Dispatcher.Invoke(new Action(() =>
            {
                if(_server != null)
                {
                    AverageUploadLabel.Content = "Up: " + sent / 333 + "KBps";
                    AverageDownloadLabel.Content = "Down: " + received / 333 + "KBps";
                    TotalUploadLabel.Content = "Total Down: " + _server.TotalBytesOut / 1000 + "KB";
                    TotalDownloadLabel.Content = "Total UP: " + _server.TotalBytesIn / 1000 + "KB";
                }
            }));
        }

        private void Hotkeys_HotkeyDown(object sender, EventArgs e)
        {
            Output("HOTKEY PRESSED");
        }

        private void ClientListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientListBox.SelectedItem is null)
            {

            }
            else if ((ClientListBox.SelectedItem as MenuItem).Tag.ToString().Split(':')[0] == "channel")
            {
                MenuItem u = (ClientListBox.SelectedItem as MenuItem);
                _server.RequestSwitchChannel(u.Tag.ToString().Split(':')[1]);
            }
        }


        private void AudioInput_DataReceieved(object sender, byte[] e)
        {
            if(_server != null && _server.Active)
            {
                if (Properties.Settings.Default.UsePTT)
                {
                    if (HotKeyManager.HotkeyDown)
                    {
                        _server.SendVoiceData(e);
                        Debug.WriteLine("Sent " + e.Length + " bytes via udp");
                    }
                }
                else
                {
                    _server.SendVoiceData(e);
                    Debug.WriteLine("Sent " + e.Length + " bytes via udp");
                }
                
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_server != null)
            {
                _server.Disconnect();
            }

            Notifications.Icon = null;
            Notifications.Dispose();


        }

        void BuildContextMenu()
        {
            System.Windows.Forms.ContextMenu _menu = new System.Windows.Forms.ContextMenu();

            //Received image list contextmenu
            System.Windows.Forms.MenuItem _imgmenu = new System.Windows.Forms.MenuItem();
            _imgmenu.Text = "Saved Images";
            foreach (Packet_ImageFromClient i in ReceivedImagesList)
            {
                System.Windows.Forms.MenuItem z = new System.Windows.Forms.MenuItem();
                z.Tag = "image:" + i.ImageName;
                z.Text = i.ImageName;
                z.Click += ContextMenuScreenshot_click;
                _imgmenu.MenuItems.Add(z);
            }

            _menu.MenuItems.Add(_imgmenu);

            //Exit button
            System.Windows.Forms.MenuItem _exit = new System.Windows.Forms.MenuItem();
            _exit.Text = "Exit";
            _exit.Click += ContextMenuExit_Click;
            _menu.MenuItems.Add(_exit);

            //Disconnect button
            if (!object.ReferenceEquals(_server, null))
            {
                System.Windows.Forms.MenuItem _disconnect = new System.Windows.Forms.MenuItem();
                _disconnect.Text = "Disconnect from " + _server.ConnectedAddress.ToString();
                _disconnect.Click += _disconnect_Click;
                _menu.MenuItems.Add(_disconnect);
            }
            Notifications.ContextMenu = _menu;
        }

        private void ContextMenuScreenshot_click(object sender, EventArgs e)
        {
            System.Windows.Forms.MenuItem s = sender as System.Windows.Forms.MenuItem;

            string _type = s.Tag.ToString().Split(':')[0];
            string _imgname = s.Tag.ToString().Split(':')[1];

            if (_type == "image")
            {
                foreach (Packet_ImageFromClient _img in ReceivedImagesList)
                {
                    if (_img.ImageName == _imgname)
                    {
                        Output("Displaying " + _img.ImageName + " from " + _img.Sender);
                        Bitmap _deserialized = (Bitmap)Serializer.DeserializeObject(_img.Image);
                        DisplayImage(_deserialized);
                    }
                }
            }

        }

        private void _disconnect_Click(object sender, EventArgs e)
        {
            _server.Disconnect();
            CloseWindow(false);
        }

        private void ContextMenuExit_Click(object sender, EventArgs e)
        {
            CloseWindow(true);
        }

        void CloseWindow(bool exit)
        {
            BuildContextMenu();
            this.Close();

            if (exit)
            {
                this.Owner.Close();
            }
        }

        private void _server_Disconnected(object sender, IPEndPoint e)
        {

            ChannelList = new Packet_ChannelList(null);
            UpdateUi();
            BuildContextMenu();
            _server = null;

            try
            {
                this.Close();
            }
            catch (Exception ex) { }
        }

        private void _server_DataReceived(object sender, ServerDataRecievedArgs e)
        {
            int _cmd = BitConverter.ToInt32(e.DataRecieved, 0);
            Debug.WriteLine("RECEIVED COMMAND " + _cmd);
            if (_cmd == (int)ServerCommand.NotifyConnected)
            {
                _server.SetLoggedIn();
                Output("Logged in...");
            }
            else if (_cmd == (int)ServerCommand.ClientAlreadyConnected)
            {
                Output("Cannot connect; already connected");
                _server.Disconnect();
            }
            else if (_cmd == (int)ServerCommand.ServerShutdown)
            {
                Output("Disconnected; Server shutting down...");
                _server.OnServerShutdown();
            }
            else if (_cmd == (int)ServerCommand.ChannelList)
            {
                byte[] _serializedlist = new byte[e.DataRecieved.Length - 4];
                Array.Copy(e.DataRecieved, 4, _serializedlist, 0, _serializedlist.Length);
                object s = Serializer.DeserializeObject(_serializedlist);
                ChannelList = s as Packet_ChannelList;
                UpdateUi();
            }
            else if (_cmd == (int)ServerCommand.MessageFromClient)
            {
                byte[] _serialized = new byte[e.DataRecieved.Length - 4];
                Array.Copy(e.DataRecieved, 4, _serialized, 0, _serialized.Length);
                Packet_MessageFromClient _msg = (Packet_MessageFromClient)Serializer.DeserializeObject(_serialized);
                Output(_msg.Dest + ": " + _msg.Message);
            }
            else if (_cmd == (int)ServerCommand.ImageFromClient)
            {
                byte[] _serialized = new byte[e.DataRecieved.Length - 4];
                Array.Copy(e.DataRecieved, 4, _serialized, 0, _serialized.Length);
                Packet_ImageFromClient _packet = (Packet_ImageFromClient)Serializer.DeserializeObject(_serialized);
                Output("Recevied " + _packet.Image.Length + " byte image from " + _packet.Sender);


                Bitmap _img = (Bitmap)Serializer.DeserializeObject(_packet.Image);
                ReceivedImagesList.Add(_packet);

                LatestImage = _img;


                Notifications.BalloonTipTitle = GetNameFromId(_packet.Sender);
                Notifications.BalloonTipText = _packet.ImageName;
                Notifications.Tag = "img";
                Notifications.ShowBalloonTip(1000);
            }else if(_cmd == (int)ServerCommand.ServerMessage)
            {
                byte[] _serialized = new byte[e.DataRecieved.Length - 4];
                Array.Copy(e.DataRecieved, 4, _serialized, 0, _serialized.Length);
                Packet_ServerMessage _packet = (Packet_ServerMessage)Serializer.DeserializeObject(_serialized);
                Output("Server: " + _packet.Message);
            }
            BuildContextMenu();

            Debug.WriteLine("ENDED DATA PARSING");
        }

        string GetNameFromId(string id)
        {
            foreach (Data_Channel i in ChannelList.AllChannels)
            {
                foreach (Data_Client c in i.ClientList)
                {
                    if (c.ID == id)
                    {
                        return c.Name;
                    }
                }
            }
            return null;
        }

        private void Notifications_BalloonTipClicked(object sender, EventArgs e)
        {
            if ((string)Notifications.Tag == "img" && LatestImage != null)
            {
                var t = new Thread(() => DisplayImage(LatestImage));
                t.SetApartmentState(ApartmentState.STA);
                t.IsBackground = true;
                t.Start();
            }
        }

        void DisplayImage(Bitmap b)
        {
            ImageForm _imgform = new ImageForm(ref b);
            _imgform.ShowDialog();

        }

        ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        private void Event_Connected(object sender, IPEndPoint e)
        {
            Output("Connected to " + e.ToString());
            DisconnectButton.Visibility = Visibility.Visible;

        }

        void Event_ConnectionFailed(object Sender, IPEndPoint Server)
        {
            _server = null;
            DisconnectButton.Visibility = Visibility.Hidden;
            Output("Failed to connect to " + Server.ToString());
            ChannelList = new Packet_ChannelList(null);
            UpdateUi();
            this.Close();
        }

        void Event_DataReceived(object sender, byte[] data)
        {
            Output("Received " + data.Length + " bytes");
        }

        void Output(string msg)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                ConsoleText.AppendText(msg);
                ConsoleText.AppendText(Environment.NewLine);
                ConsoleText.ScrollToEnd();
            }));
        }

        void UpdateUi()
        {
            ClientListBox.Dispatcher.Invoke(new Action(() =>
            {
                ClientListBox.Items.Clear();
                if (ChannelList.AllChannels != null)
                {
                    foreach (Data_Channel i in ChannelList.AllChannels.ToArray())
                    {


                        MenuItem t = new MenuItem();
                        t.Header = "---" + i.ChannelName + "---";
                        t.Tag = "channel:" + i.ChannelName;
                        t.Background = System.Windows.Media.Brushes.Gray;
                        t.ContextMenu = ChannelContextMenu();
                        ClientListBox.Items.Add(t);
                        foreach (Data_Client z in i.ClientList)
                        {
                            MenuItem e = new MenuItem();
                            e.Header = z.Name;
                            e.ContextMenu = ClientContextMenu();
                            e.Tag = "client:" + z.ID + ":" + z.Name;
                            ClientListBox.Items.Add(e);
                        }
                    }
                }
            }));
        }



        ContextMenu ClientContextMenu()
        {
            ContextMenu _menu = new ContextMenu();
            MenuItem _ItemSc = new MenuItem();
            _ItemSc.Click += ClientContextMenu_Screenshot_Click;
            _ItemSc.Header = "Send Screenshot";
            _menu.Items.Add(_ItemSc);

            return _menu;
        }

        ContextMenu ChannelContextMenu()
        {
            ContextMenu _menu = new ContextMenu();
            MenuItem _switch = new MenuItem();
            _switch.Click += ChannelContextMenu_Switch_Click;
            _switch.Header = "Switch to channel";
            _menu.Items.Add(_switch);

            return _menu;
        }

        private void ChannelContextMenu_Switch_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ClientContextMenu_Screenshot_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem t = sender as MenuItem;
            Output("Selected: " + t.Tag);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _server_ServerShutdown(object sender, IPEndPoint e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                DisconnectButton.Visibility = Visibility.Hidden;
            }));

            ChannelList = new Packet_ChannelList(null);
            UpdateUi();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _server.Disconnect();
            Output("Disconnected...");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ClientListBox.SelectedItem is null)
            {

            }
            else if ((ClientListBox.SelectedItem as MenuItem).Tag.ToString().Split(':')[0] == "client")
            {
                MenuItem u = (ClientListBox.SelectedItem as MenuItem);
                _server.SendMessage("This is a text", u.Tag.ToString().Split(':')[1]);

            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (ClientListBox.SelectedItem is null)
            {

            }
            else if ((ClientListBox.SelectedItem as MenuItem).Tag.ToString().Split(':')[0] == "client")
            {
                MenuItem u = (ClientListBox.SelectedItem as MenuItem);

                ImageManager i = new ImageManager();
                Bitmap b = i.GetScreenshot();
                _server.SendImage(b, u.Tag.ToString().Split(':')[1], Guid.NewGuid().ToString());
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (!CreateChannelTextbox.Text.Contains(':'))
            {
                _server.RequestCreateChannel(CreateChannelTextbox.Text);
            }
            
        }

        private void SwitchChannelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientListBox.SelectedItem is null)
            {

            }
            else if ((ClientListBox.SelectedItem as MenuItem).Tag.ToString().Split(':')[0] == "channel")
            {
                MenuItem u = (ClientListBox.SelectedItem as MenuItem);
                _server.RequestSwitchChannel(u.Tag.ToString().Split(':')[1]);
                Output("Requested switch to " + u.Tag.ToString().Split(':')[1]);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ClientListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.SettingsWindow _win = new Windows.SettingsWindow();
            _win.ShowDialog();
        }
    }
}
