using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkClient
{
    class DataArgs
    {
    }

    class ServerDataRecievedArgs : EventArgs
    {
        byte[] Data;

        public ServerDataRecievedArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] DataRecieved { get => Data;}
    }
}
