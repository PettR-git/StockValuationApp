using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace WTS.Utilities.Serializer
{
    public static class JsonSerialize<T>
    {
        //Serialize list
        public static void SerializeList(string fileName, List<T> list, JsonSerializerSettings options)
        {
            var jsonStr = JsonConvert.SerializeObject(list, options);
            File.WriteAllText(fileName, jsonStr);
        }

        //Deserialize list
        public static List<T> DeserializeList(string fileName, JsonSerializerSettings options)
        {
            List<T> deserializedList = null;

            var jsonStr = File.ReadAllText(fileName);
            deserializedList = JsonConvert.DeserializeObject<List<T>>(jsonStr, options);

            return deserializedList;
        }

    }
}
