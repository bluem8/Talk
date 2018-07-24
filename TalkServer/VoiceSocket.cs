using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace TalkServer
{
    class VoiceSocket
    {
        /// <summary>
        /// {GUID CLIENTID, byte[] DATA}
        /// </summary>
        public event EventHandler<object[]> DataReceived = delegate { };

        const int CLIENTSENDPORT = 8787;
        const int SERVERPORT = 1337;
        List<IPEndPoint> Clients;
        UdpClient ListenSocket;
        List<Thread> ListenThreads;

        bool Stop = false;

        public VoiceSocket()
        {
           
            ListenThreads = new List<Thread>();
            Clients = new List<IPEndPoint>();

            Thread t  = new Thread(ListenLoop);
            t.Start();
        }

        public List<IPEndPoint> AllPorts { get => Clients;}

        public void SendToClient(IPEndPoint client, byte[] data)
        {
            ListenSocket.Send(data, data.Length, client);
            Console.WriteLine("Sent voice data to " + client.ToString());
            //Debug.WriteLine("Sent " + data.Length + " bytes to " + client.ToString() + " from " + ListenSocket.Client.LocalEndPoint);
        }

        public void AddClient(Client c)
        {
            /*
            IPEndPoint _dest = new IPEndPoint(c.ClientAddress, CLIENTSENDPORT);
            Thread _rec = new Thread(() => ListenLoop(_dest, c));
            ListenThreads.Add(_rec);
            Clients.Add(_dest);
            _rec.Start();*/
        }

        public void RemoveClient(Client c)
        {/*
            int i = 0;
            foreach(IPEndPoint e in Clients.ToArray())
            {
                if (e.Address == c.ClientAddress)
                {
                    ListenThreads[i].Abort();
                    ListenThreads.RemoveAt(i);
                    Clients.RemoveAt(i);
                    Debug.WriteLine("REMOVED " + i);
                }
                i++;
            }*/
        }

        void ParseInput(byte[] data)
        {
            if (data.Length != 0)
            {
                byte[] _header = new byte[36];
                Array.Copy(data, 0, _header, 0, 36);

                Guid sender = new Guid(Encoding.ASCII.GetString(_header));
                Console.WriteLine("voice data from " + sender);

                byte[] _audiodata = new byte[data.Length - 36];
                Array.Copy(data, 36, _audiodata, 0, data.Length - 36);
                DataReceived(this, new object[2] { sender, data });
            }
        }

        void ListenLoop()
        {
            ListenSocket = new UdpClient(SERVERPORT);
            IPEndPoint _any = new IPEndPoint(IPAddress.Any, 0);
            while (!Stop)
            {
                //Debug.WriteLine("Waiting for data from " + _client.ToString());
                byte[] data = ListenSocket.Receive(ref _any);
                Console.WriteLine("received " + data.Length + " bytes");

                var t = new Thread(() => ParseInput(data));
                t.Start();

                //e.AddBytes(data);
                //Debug.WriteLine("added " + data.Length +" bytes from " + _client.ToString());
            }
        }
    }
}
