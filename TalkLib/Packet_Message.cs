using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    [Serializable]
    public class Packet_Message
    {
        string dest;
        string message;

        public Packet_Message(string msg, string destclient)
        {
            dest = destclient;
            message = msg;
        }

        public string Dest { get => dest;}
        public string Message { get => message;}
    }
}
