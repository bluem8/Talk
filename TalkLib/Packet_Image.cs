using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    [Serializable]
    public class Packet_Image
    {
        string dest;
        byte[] image;
        string Imagename;

        public Packet_Image(byte[] img, string destclient, string imgname)
        {
            Imagename = imgname;
            dest = destclient;
            image = img;
        }

        public string Dest { get => dest; }
        public byte[] Image { get => image; }
        public string ImageName { get => Imagename; set => Imagename = value; }
    }
}
