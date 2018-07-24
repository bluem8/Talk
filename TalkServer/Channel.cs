using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkServer
{
    class Channel
    {
        string Name;
        int Id;
        List<Client> Clients;

        public Channel(string name, int id)
        {
            Clients = new List<Client>();
            Name = name;
            Id = id;
        }

        public string ChannelName { get => Name;}
        public int ChannelId { get => Id;}
        internal List<Client> ClientList { get => Clients;}
    }
}
