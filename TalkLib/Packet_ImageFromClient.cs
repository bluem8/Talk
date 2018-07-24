using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    [Serializable]
    public class Packet_ImageFromClient
    {
        string sender;
        byte[] image;
        string imgname;

        public Packet_ImageFromClient(byte[] img, string sentby, string name)
        {
            imgname = name;
            sender = sentby;
            image = img;
        }

        public string Sender { get => sender; }
        public byte[] Image { get => image; }
        public string ImageName { get => imgname;}
    }
}
