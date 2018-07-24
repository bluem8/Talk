using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkServer
{
    class ChannelManager
    {
        public event EventHandler Updated = delegate { };
        
        List<Channel> ChannelList;
        Random IdGen;

        Channel Default;

        internal List<Channel> Channels { get => ChannelList;}
        internal Channel DefaultChannel { get => Default; set => Default = value; }

        public ChannelManager()
        {
            IdGen = new Random();
            ChannelList = new List<Channel>();
        }

        /// <summary>
        /// Creates a new channel
        /// </summary>
        /// <param name="name"></param>
        public void CreateChannel(string name)
        {
            if (!IsNameTaken(name))
            {
                Channel n = new Channel(name, IdGen.Next(0, int.MaxValue));
                ChannelList.Add(n);
            }
        }

        public bool MoveClient(Client client, Channel dest)
        {
            try
            {
                if (GetChannelFromClient(client) != null)
                {
                    GetChannelFromClient(client).ClientList.Remove(client);

                }
                dest.ClientList.Add(client);
                client.ClientChannel = dest;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Move client failed " + e.Message);
                return false;
            }

        }
        public bool RenoveClient(Client client)
        {
            try
            {
                if (GetChannelFromClient(client) != null)
                {
                    GetChannelFromClient(client).ClientList.Remove(client);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to remove client" + e.Message);
                return false;
            }

        }

        /// <summary>
        /// Checks if a channel name exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsNameTaken(string name)
        {
            foreach (Channel i in ChannelList)
            {
                if (i.ChannelName == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds client current channel
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public Channel GetChannelFromClient(Client client)
        {
            foreach (Channel i in ChannelList)
            {
                foreach(Client c in i.ClientList)
                {
                    if(c == client)
                    {
                        return i;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Finds a channel from the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Channel GetChannelFromId(int id)
        {
            foreach (Channel i in ChannelList)
            {
                if (i.ChannelId == id)
                {
                    return i;
                }
            }

            return null;
        }

        public Channel GetChannelFromName(string name)
        {
            foreach (Channel i in ChannelList)
            {
                if (i.ChannelName == name)
                {
                    return i;
                }
            }
            return null;
        }
    }
}
