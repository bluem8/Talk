using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    [Serializable]
    public class Packet_ChannelList
    {
        Data_Channel[] Channels;

        public Packet_ChannelList(Data_Channel[] chans)
        {
            Channels = chans;
        }

        public Data_Channel[] AllChannels { get => Channels;}
    }
}
