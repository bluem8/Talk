using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkServer
{
    class ClientManager
    {
        List<Client> ClientList = new List<Client>();

        internal List<Client> AllClients { get => ClientList; set => ClientList = value; }

        /// <summary>
        /// Finds a client from specified GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Client GetClientByGuid(Guid id)
        {
            foreach(Client i in ClientList)
            {
                if (i.Id == id)
                {
                    return i;
                }
            }
            return null;
        }

        public void AddClient(Client c)
        {
            if(GetClientByGuid(c.Id) == null){
                ClientList.Add(c);
            }
            else
            {
                Debug.WriteLine("Duplicate client {" + c.Id.ToString() + "}");
            }
        }

        public void RemoveClient(Client c)
        {
            ClientList.Remove(c);
        }
    }
}
