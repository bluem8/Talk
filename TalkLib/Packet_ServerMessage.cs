using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    [Serializable]
    public class Packet_ServerMessage
    {
        public string Message;

        public Packet_ServerMessage(string msg)
        {
            Message = msg;
        }
    }
}
