using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    [Serializable]
    public class Data_Client
    {
        string name;
        string id;

        public Data_Client(string username, string cid)
        {
            name = username;
            id = cid;
        }

        public string Name { get => name;}
        public string ID { get => id;}
    }
}
