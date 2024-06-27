using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using WTS.Entities;
using WTS.Utilities.Serializer;
using System.Runtime;
using System.Windows.Documents;


namespace WTS.Entities.Main
{
    public class ListManager<T>
    {
        private List<T> list;
        public ListManager()
        {
            list = new List<T>();
        }

        public bool addItem(T item)
        {
            bool ok = false;
            if (item != null) {
                list.Add(item);
                ok = true;
            }

            return ok;
        }

        public bool removeItem(T item)
        {
            bool ok = false;
            if (checkIndex(list.IndexOf(item)))
            {
                list.Remove(item);
                ok = true;
            }
            return ok;
        }

        public bool changeItem(T item, int index)
        {
            bool ok = false;

            if (checkIndex(index))
            {
                list[index] = item;
                ok = true;
            }

            return ok;
        }

        private bool checkIndex(int index)
        {
            bool ok = false;
            if (index <= list.Count && index >= 0)
            {
                ok = true;
            }
            return ok;
        }

        //Get item at specific index of list
        public T getListItemAt(int index)
        {
            T item;

            if (checkIndex(index) == false)
                return default(T);

            item = list[index];

            return item;
        }

        //Get all item ToStrings as string array
        public string[] getListToStrings()
        {
            string[] itemArr = new string[list.Count];

            int count = 0;

            foreach (T item in list)
            {
                itemArr[count++] = item.ToString();
            }

            return itemArr;
        }

        //Default sort, initialized by class of obj T
        public void defaultSort()
        {
            list.Sort();
        }

        //Sort based on obj T
        public void secondarySort(Comparison<T> comparison)
        {
            list.Sort(comparison);
        }

        public bool binaryDeSerialize(string fileName)
        {
            List<T> tempList = BinarySerialize<T>.DeserializeList(fileName);
            list.AddRange(tempList);
            if (list.Count > 0)
                return true;

            return false;
        }

        public int Count() {  return list.Count; }

        public void binarySerialize(string fileName)
        {
            BinarySerialize<T>.SerializeList(fileName, list);      
        }

        public void jsonSerialize(string fileName, JsonSerializerSettings options = null)
        {
           JsonSerialize<T>.SerializeList(fileName, list, options);
        }

        public bool jsonDeSerialize(string fileName, JsonSerializerSettings options)
        {
            List<T> tempList = JsonSerialize<T>.DeserializeList(fileName, options);
            list.AddRange(tempList);
            if (list.Count > 0)
                return true;

            return false;
        }

        public void xmlSerialize(string fileName)
        {
            XMLSerialize<T>.SerializeList(fileName, list);
        }

        public bool xmlDeserialize(string fileName)
        {
            List<T> tempList = XMLSerialize<T>.DeserializeList(fileName);
            list.AddRange(tempList);
            if (list.Count > 0)
                return true;

            return false;
        }

    }
}
