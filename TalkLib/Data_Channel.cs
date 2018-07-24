using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    [Serializable]
    public class Data_Channel
    {
        string Name;
        int Id;
        Data_Client[] Clients;

        public Data_Channel(string name, int id, Data_Client[] clients)
        {
            Clients = clients;
            Name = name;
            Id = id;
        }

        public string ChannelName { get => Name; }
        public int ChannelId { get => Id; }
        public Data_Client[] ClientList { get => Clients; }
    }
}
