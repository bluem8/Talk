using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TalkClient
{
    class VoiceSocket
    {
        public event EventHandler<IPEndPoint> Connected = delegate { };


        UdpClient UdpSocket;

        ServerConnection Owner;

        IPEndPoint DestSocket;

        Thread DataThread;

        List<Debugging.TempPlayback> DataOutputs = new List<Debugging.TempPlayback>();

        bool Stop = false;

        public VoiceSocket(ServerConnection _con)
        {
            DestSocket = new IPEndPoint(_con.ConnectedAddress.Address, 1337);
            Owner = _con;
            UdpSocket = new UdpClient();
            UdpSocket.Client.Bind(new IPEndPoint(IPAddress.Any, 8787));
            UdpSocket.Connect(DestSocket);
            DataThread = new Thread(Wait);
            DataThread.Start();
        }

        void Wait()
        {
            while (!Stop)
            {
                try
                {
                    byte[] data = UdpSocket.Receive(ref DestSocket);
                    string _sender = System.Text.Encoding.Default.GetString(data, 0, 36);
                    Console.WriteLine("received audio from " + _sender);

                    byte[] _audiodata = new byte[data.Length - 36];
                    Array.Copy(data, 36, _audiodata, 0, data.Length - 36);

                    bool found = false;
                    foreach (Debugging.TempPlayback i in DataOutputs)
                    {
                        if (i.Owner == _sender)
                        {
                            found = true;
                            i.AddBytes(_audiodata);
                        }
                    }

                    if (!found)
                    {
                        Debugging.TempPlayback z = new Debugging.TempPlayback(_sender);
                        DataOutputs.Add(z);
                    }

                }
                catch (Exception ex) { }
            }

        }
        public void SendData(byte[] data)
        {
            if(UdpSocket != null)
            {
                try
                {
                    byte[] _header = Encoding.ASCII.GetBytes(Owner.Id.ToString());
                    byte[] _fulldata = new byte[data.Length + 36];

                    Array.Copy(_header, 0, _fulldata, 0, 36);
                    Array.Copy(data, 0, _fulldata, 36, data.Length);
                    
                    UdpSocket.Send(_fulldata, _fulldata.Length);
                    //Console.WriteLine(data.Length + "|" + Encoding.ASCII.GetByteCount(Owner.Id.ToString()) + "|" + _fulldata.Length);
                    //Console.WriteLine("Sent " + _fulldata.Length);
                }
                catch(Exception e)
                {
                    Debug.WriteLine("ATTEMPTED TO USE DISPOSED SOCKET " + e.Message);
                }

            }
        }

        public void Shutdown()
        {
            Stop = true;
            DataThread.Abort();
            UdpSocket.Dispose();

            foreach(Debugging.TempPlayback z in DataOutputs)
            {
                z.Exit();
            }
        }
    }
}
