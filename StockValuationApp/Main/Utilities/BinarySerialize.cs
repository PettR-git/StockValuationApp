using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WTS.Utilities.Serializer
{
    public static class BinarySerialize<T>
    {
        //Serialize list
        public static void SerializeList(string fileName, List<T> list)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fileStream, list);
                fileStream.Close();
            }
        }

        //Deserialize list
        public static List<T> DeserializeList(string fileName)
        {
            List<T> objs = new List<T>();

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                BinaryFormatter b = new BinaryFormatter();
                objs = (List<T>)b.Deserialize(fileStream);
                fileStream.Close();
            }

            return objs;
        }
    }
}
