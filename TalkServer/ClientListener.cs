using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TalkServer
{
    class ClientListener
    {
        public event EventHandler<int> Started = delegate { };
        public event EventHandler<int> Stopped = delegate { };

        public event EventHandler<TcpClient> ClientAccepted = delegate { };

        public event EventHandler<byte[]> DataSent = delegate { };
        public event EventHandler<byte[]> DataReceived = delegate { };

        /// <summary>
        /// TcpListener to wait accept tcpclients
        /// </summary>
        TcpListener TcpL;

        /// <summary>
        /// Is clientlistener active
        /// </summary>
        bool Listening;

        /// <summary>
        /// Thread for listening for clients
        /// </summary>
        Thread ListenThread;

        /// <summary>
        /// Set true to stop listening for clients
        /// </summary>
        bool StopListening = false;

        /// <summary>
        /// Port to listen on
        /// </summary>
        int ListenPort;

        /// <summary>
        /// Is TcpListener waiting for clients?
        /// </summary>
        public bool Active { get => Listening;}

        /// <summary>
        /// Start listening for clients
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            ListenPort = port;
            ListenThread = new Thread(Listen);
            ListenThread.Start();
        }

        void Listen()
        {
            TcpL = new TcpListener(IPAddress.Any, ListenPort);
            TcpL.Start(ListenPort);
            Listening = true;
            Started(this, ListenPort);
            while (!StopListening)
            {
                TcpClient _client = TcpL.AcceptTcpClient();
                ClientAccepted(this, _client);
            }
            Listening = false;
            Stopped(this, ListenPort);
        }

        public void Stop()
        {
            try
            {
                StopListening = true;
                TcpL.Stop();
                ListenThread.Abort();
            }catch(Exception e)
            {

            }
        }
    }
}
