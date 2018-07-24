using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

namespace TalkServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        ClientListener Clients;
        ChannelManager Channels;
        ClientManager ClientList;
        VoiceSocket VoiceServer;

        Thread UiThread;
        public MainWindow()
        {
            AllocConsole();
            VoiceServer = new VoiceSocket();
            VoiceServer.DataReceived += VoiceServer_DataReceived;
            ClientList = new ClientManager();
            this.Closing += MainWindow_Closed;
            InitializeComponent();
            Channels = new ChannelManager();
            Channels.CreateChannel("Test");
            Channels.DefaultChannel = Channels.GetChannelFromName("Test");
            Channels.Updated += Channels_Updated;

            Clients = new ClientListener();
            Clients.ClientAccepted += Clients_ClientAccepted;
            Clients.Stopped += Clients_Stopped;
            Clients.Started += Clients_Started;
            Clients.Start(4567);

            ConsoleTextbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            ConsoleTextbox.IsReadOnly = true;
            ConsoleTextbox.Background = Brushes.Black;
            ConsoleTextbox.Foreground = Brushes.Green;
            ConsoleTextbox.Document.Blocks.Clear();
            //ConsoleTextbox.Margin = new Thickness(0);
        }

        private void VoiceServer_DataReceived(object sender, object[] e)
        {
            Guid _client = (Guid)e[0];
            Console.WriteLine("Input from " + _client);
            byte[] _data = (byte[])e[1];
            Client c = ClientList.GetClientByGuid(_client);
            Console.WriteLine("DATA INPUT FROM " + c.Username);

            foreach(Client i in c.ClientChannel.ClientList)
            {
                if(i.Id != c.Id)
                {
                    VoiceServer.SendToClient(i.VoiceAddress, _data);
                    Console.WriteLine(c.Username + "=>" + i.Username);
                }
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
               // Clients.Stop();


            foreach (Client i in ClientList.AllClients.ToArray())
            {
                try
                {
                    Clients.Stop();
                }catch(Exception ex)
                {

                }

                try
                {
                    i.NotifyServerShutdown();
                    ClientList.RemoveClient(i);
                    i.Shutdown();   
                }catch(Exception ex)
                {

                }


            }
        }

        private void Channels_Updated(object sender, EventArgs e)
        {
            Update();

            

        }

        void UpdateUi()
        {
            try
            {
                ClientListBox.Dispatcher.Invoke((Action)delegate
                {
                    Update();
                });
            }catch(Exception e)
            {

            }

        }
        
        void Update()
        {
            List<MenuItem> _items = new List<MenuItem>();
            foreach (Channel _channel in Channels.Channels)
            {
                MenuItem t = new MenuItem();
                t.Header = "---" + _channel.ChannelName + "---";
                t.Tag = "channel:" + _channel.ChannelName;
                _items.Add(t);
                foreach (Client i in _channel.ClientList)
                {
                    MenuItem x = new MenuItem();
                    x.Header = i.Username;
                    x.Tag = "client:" + i.Username;
                    _items.Add(x);
                }
            }
            ClientListBox.Items.Clear();
            foreach (MenuItem i in _items)
            {
                ClientListBox.Items.Add(i);
            }

            foreach(Client i in ClientList.AllClients)
            {
                i.SendChannelList(Channels.Channels);
            }
        }

        private void Clients_Started(object sender, int e)
        {
            Print("Started listening in port " + e);
        }

        private void Clients_Stopped(object sender, int e)
        {
            Print("Stopped listening in port " + e);
        }

        void Clients_ClientAccepted(object sender, System.Net.Sockets.TcpClient e)
        {
            Print("Accepted client from " + e.Client.RemoteEndPoint.ToString());
            UnknownClient _client = new UnknownClient(e);
            _client.ClientRegistered += _client_ClientRegistered;
        }

        private void _client_ClientRegistered(object sender, string[] e)
        {
            UnknownClient _client = (sender as UnknownClient);
            Print(e[0] + " logged in (" + _client.ClientAddress + ") (" + e[1] + ")");

            if (ClientList.GetClientByGuid(new Guid(e[1])) == null){
                Client i = new Client(_client.Socket, e[0], new Guid(e[1]));
                Channels.MoveClient(i, Channels.DefaultChannel);
                i.ClientDisconnected += I_ClientDisconnected;
                i.SentMessage += I_SentMessage;
                i.SentImage += I_SentImage;
                i.RequestedCreateChannel += I_RequestedCreateChannel;
                i.RequestedSwitchChannel += I_RequestedSwitchChannel;
                i.NotifyLogin();
                ClientList.AddClient(i);
                VoiceServer.AddClient(i);
                BroadcastMessage(i.Username + " connected...");
                UpdateUi();
            }
            else
            {
                _client.SendCommand(TalkLib.ServerCommand.ClientAlreadyConnected);
            }
        }

        public void BroadcastMessage(string msg)
        {
            foreach(Client i in ClientList.AllClients)
            {
                i.SendGlobalServerMessage(msg);
            }
        }

        private void I_RequestedSwitchChannel(object sender, string e)
        {
            //TODO - CHECK PERMS
            Client _c = sender as Client;
            Channel _ch = Channels.GetChannelFromName(e);
            Debug.WriteLine("SERVER; " + _c.Username + " SWITCHING TO " + _ch.ChannelId);
            BroadcastMessage(_c.Username + " moved to " + _ch.ChannelName);
            Channels.MoveClient(_c, _ch);
            UpdateUi();
        }

        private void I_RequestedCreateChannel(object sender, string e)
        {
            //TODO - CHECK PERMS

            Channels.CreateChannel(e);
            UpdateUi();
        }

        private void I_SentImage(object sender, object[] e)
        {
            string _dest = (string)e[0];
            byte[] imgdata = (byte[])e[1];
            string _name = (string)e[2];
            Client i = sender as Client;

            Print(i.Id + " Sent image {" + imgdata.Length + " bytes} to " + _dest);

            Client _destclient = ClientList.GetClientByGuid(new Guid(_dest));
            _destclient.ImageFromClient(imgdata, i.Id.ToString(), _name);
        }

        private void I_SentMessage(object sender, string[] e)
        {
            Client i = sender as Client;
            Print(i.Id + " '" + e[0] + "' -> " + e[1]);

            Client dest = ClientList.GetClientByGuid(new Guid(e[1]));
            dest.MessageFromClient(e[0], e[1]);
        }

        private void I_ClientDisconnected(object sender, bool e)
        {
            Client i = sender as Client;
            VoiceServer.RemoveClient(i);
            bool _notify = false;

            foreach(Client c in ClientList.AllClients)
            {
                if(c == i)
                {
                    _notify = true;
                }
            }
            if (_notify)
            {
                Print(i.Username + " disconnected");
                ClientList.RemoveClient(i);
                Channels.RenoveClient(i);
                UpdateUi();
            }


        }

        void Print(string msg)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    ConsoleTextbox.AppendText(msg);
                    ConsoleTextbox.AppendText(Environment.NewLine);
                }));
            }catch(Exception E)
            {

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (Client i in ClientList.AllClients)
            {
                i.SendChannelList(Channels.Channels);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BroadcastMessage("this is a test");
        }
    }
}
