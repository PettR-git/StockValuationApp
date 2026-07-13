using Newtonsoft.Json;
using System;
using System.Collections;
using WTS.Entities;
using WTS.Utilities.Serializer;
using Formatting = Newtonsoft.Json.Formatting;


namespace WTS.Entities.Main
{
    public class ListManager<T> : IEnumerable<T>
    {
        private List<T> list;
        public ListManager()
        {
            list = new List<T>();
        }

        // Existing API (preserved)
        public bool addItem(T item)
        {
            bool ok = false;
            if (item != null)
            {
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

        public int Count() { return list.Count; }

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

        // Added list-like surface so consumers (like StockManager) can call Clear/AddRange/etc.
        public void Clear() => list.Clear();

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) return;
            list.AddRange(items);
        }

        public void Add(T item) => list.Add(item);

        public bool Remove(T item) => list.Remove(item);

        // Enumeration support so extension methods like ToList() work on 'this'
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();

        // Convenience accessor
        public IEnumerable<T> AsEnumerable() => list.AsEnumerable();
    }
}
