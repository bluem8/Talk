using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    [Serializable]
    public class Packet_MessageFromClient
    {
        string Sender;
        string message;

        public Packet_MessageFromClient(string msg, string send)
        {
            Sender = send;
            message = msg;
        }

        public string Dest { get => Sender; }
        public string Message { get => message; }
    }
}
