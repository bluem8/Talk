using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Diagnostics;
using TalkLib;
using System.Drawing;
using System.Windows;

namespace TalkClient
{
    class ServerConnection
    {
        System.Windows.Forms.Timer BpsTimer;

        public event EventHandler<IPEndPoint> Connected = delegate { };
        public event EventHandler<IPEndPoint> Disconnected = delegate { };
        public event EventHandler<IPEndPoint> ConnectionFailed = delegate { };
        public event EventHandler<IPEndPoint> ConnectionError = delegate { };

        public event EventHandler<IPEndPoint> ServerShutdown = delegate { };

        public event EventHandler<ServerDataRecievedArgs> DataReceived = delegate { };

        public event EventHandler<object[]> BpsUpdated = delegate { };

        /// <summary>
        /// Socket used to connnect
        /// </summary>
        TcpClient TcpSocket;

        /// <summary>
        /// Destination server address
        /// </summary>
        IPEndPoint DestinationServer;

        /// <summary>
        /// Is the client connected
        /// </summary>
        bool Isconnected = false;

        /// <summary>
        /// Thread used to listen for data
        /// </summary>
        Thread ListenThread;

        /// <summary>
        /// Enable to stop connection
        /// </summary>
        bool StopConnection = false;

        /// <summary>
        /// Reads data from the TcpClient
        /// </summary>
        BinaryReader BinRead;

        /// <summary>
        /// Writes data to the binarystream
        /// </summary>
        BinaryWriter BinWrite;

        long TotalBytesSent = 0;
        long TotalBytesReceived = 0;
        long BytesSent = 0;
        long BytesReceived = 0;

        Guid ThisId;

        /// <summary>
        /// Is client logged in
        /// </summary>
        bool IsLoggedIn = false;

        IPEndPoint ServerAddress;

        /// <summary>
        /// UDP socket for sending voice data
        /// </summary>
        VoiceSocket VoiceConnection;

        public bool Active { get => Isconnected;}
        public IPEndPoint ConnectedAddress { get => ServerAddress;}
        public Guid Id { get => ThisId;}
        public long TotalBytesOut { get => TotalBytesSent; set => TotalBytesSent = value; }
        public long TotalBytesIn { get => TotalBytesReceived; set => TotalBytesReceived = value; }

        /// <summary>
        /// Connect to specified server
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Connect(IPEndPoint dest, Guid Gid, string username)
        {
            if (Isconnected)
            {
                return false;
            }
            else
            {
                try
                {
                    ThisId = Gid;
                    DestinationServer = dest;
                    TcpSocket = new TcpClient();
                    TcpSocket.Connect(dest);
                    ServerAddress = dest;
                    BinRead = new BinaryReader(TcpSocket.GetStream());
                    BinWrite = new BinaryWriter(TcpSocket.GetStream());

                    ListenThread = new Thread(ListenForData);
                    ListenThread.Start();
                    SendInitialData(username, Gid);
                    Isconnected = true;
                    SetupUdp();
                    Connected(this, (TcpSocket.Client.RemoteEndPoint as IPEndPoint));
                    BpsTimer = new System.Windows.Forms.Timer();
                    BpsTimer.Interval = 333;
                    BpsTimer.Tick += BpsTimer_Tick;
                    BpsTimer.Start();

                    return true;
                }catch(SocketException e)
                {
                    Isconnected = false;
                    MessageBox.Show(e.Message);
                    ConnectionFailed(this, DestinationServer);
                    return false;
                }
            }
        }

        private void BpsTimer_Tick(object sender, EventArgs e)
        {
            BpsUpdated(this, new object[2] { BytesSent, BytesReceived });
            TotalBytesSent += BytesSent;
            TotalBytesReceived += BytesReceived;
            BytesReceived = 0;
            BytesSent = 0;
        }

        void CheckConnection()
        {
            if(!TcpSocket.Connected)
            {
                Debug.WriteLine("Connection lost");
            }
        }

        void SetupUdp()
        {
            VoiceConnection = new VoiceSocket(this);
            VoiceConnection.Connected += VoiceConnection_Connected;
        }

        public void SendVoiceData(byte[] data)
        {
            if(VoiceConnection != null && Active)
            {
                VoiceConnection.SendData(data);
                BytesSent += data.Length;
            }
        }

        private void VoiceConnection_Connected(object sender, IPEndPoint e)
        {
            Debug.WriteLine("UDP connected.");
        }

        public void SendMessage(string msg, string destid)
        {
            SendCommand(ClientCommand.SendMessage, msg, destid);
        }

        public void RequestCreateChannel(string channelname)
        {
            SendCommand(ClientCommand.CreateChannel, channelname);
        }

        void ListenForData()
        {
            while (!StopConnection)
            {
                try
                {
                    Debug.WriteLine("TalkClient: Waiting for data");
                    int _size = BinRead.ReadInt32();
                    byte[] _data = new byte[_size];
                    _data = BinRead.ReadBytes(_size);
                    Debug.WriteLine("CLIENT: " + _data.Length + " bytes");
                    ServerDataRecievedArgs _args = new ServerDataRecievedArgs(_data);
                    BytesReceived += _size;
                    DataReceived(null, _args);
                }catch(Exception e)
                {
                    if(e is System.IO.IOException)
                    {
                        ConnectionError(this, ConnectedAddress);
                    }
                    Debug.WriteLine(e.Message);
                    StopConnection = true;
                }

            }
        }

        public void Shutdown(int reason)
        {
            if(reason == 0)
            {
                Disconnected(this, ServerAddress);
            }else if(reason == 1)
            {
                ServerShutdown(this, ServerAddress);
            }
            
        }
        void SendCommand(ClientCommand command, params object[] args)
        {
            try
            {
                if (command == ClientCommand.Disconnect)
                {
                    BinWrite.Write(4);
                    BinWrite.Write((int)command);
                }else if(command == ClientCommand.SendMessage)
                {
                    Packet_Message a = new Packet_Message(args[0] as string, (string)args[1]);
                    byte[] data = Serializer.SerializeObject(a);
                    BinWrite.Write(4 + data.Length);
                    BinWrite.Write((int)command);
                    BinWrite.Write(data);
                }else if(command == ClientCommand.SendImage)
                {
                    byte[] _data = (byte[])args[0];
                    string _dest = (string)args[1];

                    Packet_Image i = new Packet_Image(_data, _dest, (string)args[2]);
                    Debug.WriteLine("CLIENT - SENDING IMAGE TO " + _dest);
                    byte[] _packet = Serializer.SerializeObject(i);
                    BinWrite.Write(4 + _packet.Length);
                    BytesSent += 4 + _packet.Length;
                    BinWrite.Write((int)command);
                    BinWrite.Write(_packet);
                }else if(command == ClientCommand.CreateChannel)
                {
                    BinWrite.Write(4);
                    BinWrite.Write((int)command);
                    BinWrite.Write((string)args[0]);
                }else if(command == ClientCommand.SwitchChannel)
                {
                    BinWrite.Write(4);
                    BinWrite.Write((int)command);
                    BinWrite.Write((string)args[0]);
                }
            }catch(Exception e){
                Debug.WriteLine("Error sending command " + e.Message);
            }
        }

        public void SendImage(Bitmap img, string destid, string name)
        {
            byte[] _data = Serializer.SerializeObject(img);
            SendCommand(ClientCommand.SendImage, _data, destid, name);

        }

        void SendInitialData(string name, Guid Gid)
        {
            BinWrite.Write(name.Length);
            BinWrite.Write(Gid.ToString().Length);

            byte[] _data;
            using (MemoryStream ms = new MemoryStream())
            {
                //Write login header
                byte[] header = new byte[3];
                header[0] = 144;
                header[1] = 211;
                header[2] = 121;
                ms.Write(header, 0, 3);

                //Write name array
                byte[] _name = Encoding.UTF8.GetBytes(name);

                //Write UID
                byte[] _id = Encoding.UTF8.GetBytes(Gid.ToString());

                ms.Write(_name, 0, _name.Length);
                ms.Write(_id, 0, _id.Length);
                _data = ms.ToArray();
            }

            BinWrite.Write(_data);

            Debug.WriteLine("Client: Sent data");
        }
        
        public void Disconnect()
        {
            if (TcpSocket.Connected)
            {
                SendCommand(ClientCommand.Disconnect);
                TcpSocket.Client.Disconnect(false);
            }
            VoiceConnection.Shutdown();
            Disconnected(this, ServerAddress);
            Isconnected = false;
        }

        public void RequestSwitchChannel(string channelname)
        {
            SendCommand(ClientCommand.SwitchChannel, channelname);
        }

        public void OnServerShutdown()
        {
            ServerShutdown(this, ServerAddress);
        }

        public void SetLoggedIn()
        {
            IsLoggedIn = true;
        }
    }
}
