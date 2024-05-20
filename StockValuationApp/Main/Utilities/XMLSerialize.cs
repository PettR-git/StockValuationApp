using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Xml.Serialization;

namespace WTS.Utilities.Serializer
{
    public static class XMLSerialize<T>
    {
        //Serialize list
        public static void SerializeList(string fileName, List<T> list)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, list);
                writer.Close();
            }   
        }

        //Deserialize list
        public static List<T> DeserializeList(string fileName)
        {
            List<T> deserializedList = null;

            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

            using (FileStream reader = new FileStream(fileName, FileMode.Open))
            {
                deserializedList = (List<T>)serializer.Deserialize(reader);
                reader.Close();
            }

            return deserializedList;
        }

    }
}
