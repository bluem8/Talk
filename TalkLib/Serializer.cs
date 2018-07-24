using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TalkLib
{
    public static class Serializer
    {
        static BinaryFormatter BinF = new BinaryFormatter();

        public static byte[] SerializeObject(object i)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                BinF.Serialize(ms, i);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public static object DeserializeObject(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return BinF.Deserialize(ms);
            }
        }
    }
}

