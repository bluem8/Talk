using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TalkLib;

namespace TalkServer
{
    class UnknownClient
    {

        /// <summary>
        /// Called when client loses connection
        /// </summary>
        public event EventHandler<string> ClientDisconnected = delegate { };
        
        /// <summary>
        /// Raised when the client sends a username
        /// </summary>
        public event EventHandler<string[]> ClientRegistered = delegate { };


        /// <summary>
        /// Client socket
        /// </summary>
        TcpClient TcpC;

        /// <summary>
        /// Is TCP client connected
        /// </summary>
        bool IsActive = false;

        /// <summary>
        /// Data listen thread
        /// </summary>
        Thread ListenThread;

        /// <summary>
        /// Disconnects client when true
        /// </summary>
        bool RemoveClient = false;

        /// <summary>
        /// Binary Stream reader
        /// </summary>
        BinaryReader BinRead;

        /// <summary>
        /// Client address
        /// </summary>
        IPAddress Address;

        BinaryWriter BinWrite;

        public IPAddress ClientAddress{ get => Address;}
        public bool Active { get => IsActive;}
        public TcpClient Socket { get => TcpC;}

        /// <summary>
        /// Creates a new unknown client and sets up listen thread
        /// </summary>
        /// <param name="Client"></param>
        public UnknownClient(TcpClient Client)
        {
            TcpC = Client;
            IsActive = true;
            BinWrite = new BinaryWriter(TcpC.GetStream());
            BinRead = new BinaryReader(TcpC.GetStream());
            Address = (TcpC.Client.RemoteEndPoint as IPEndPoint).Address;
            ListenThread = new Thread(Listen);
            ListenThread.Start();
        }

        /// <summary>
        /// Waits for data
        /// </summary>
        void Listen()
        {
            try
            {
                while (!RemoveClient)
                {
                    int _namesize = BinRead.ReadInt32();
                    int _idsize = BinRead.ReadInt32();
                    byte[] _data = BinRead.ReadBytes(3 + _namesize + _idsize);
                    ParseData(_data, _namesize, _idsize);
                }
            }catch(SocketException e)
            {
                IsActive = false;
                Debug.WriteLine("Server: Client socket error " + e.Message);
                ClientDisconnected(this, e.Message);
            }

        }

        /// <summary>
        /// Parses incoming username data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        void ParseData(byte[] data, int namesize, int idsize)
        {
            if(data[0] == 144 && data[1] == 211 && data[2] == 121)
            {
                string _name = Encoding.UTF8.GetString(data, 3, namesize);
                string _id = Encoding.UTF8.GetString(data, 3 + namesize, idsize);
                Debug.WriteLine(_name + ":" + _id);
                ClientRegistered(this, new string[] { _name, _id });
                ShutDown();
            }
        }
        /// <summary>
        /// Sends command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public void SendCommand(ServerCommand command, params object[] args)
        {
            if (command == ServerCommand.ClientAlreadyConnected)
            {
                BinWrite.Write(4);
                BinWrite.Write((int)ServerCommand.ClientAlreadyConnected);
            }
        }
        /// <summary>
        /// Cleans up after client logs in
        /// </summary>
        public void ShutDown()
        {
            RemoveClient = true;
            IsActive = false;
            ListenThread.Abort();
        }
    }
}
