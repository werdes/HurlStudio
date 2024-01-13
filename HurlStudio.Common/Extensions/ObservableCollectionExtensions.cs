using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddIfNotNull<T>(this ObservableCollection<T> source, T? obj)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (obj == null) return;
            source.Add(obj);
        }

        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                source.Add(value);
            }
        }

        public static void AddRangeIfNotNull<T>(this ObservableCollection<T> source, IEnumerable<T>? values)
        {
            if (values != null)
            {
                foreach (T value in values)
                {
                    source.Add(value);
                }
            }
        }

        public static void InsertIfNotContains<T>(this ObservableCollection<T> source, int index, T item)
        {
            if (!source.Contains(item))
            {
                source.Insert(index, item);
            }
        }

        public static void RemoveAll<T>(this ObservableCollection<T> source, Func<T, bool> predicate)
        {
            List<T> itemsForRemoval = new List<T>();
            foreach (T element in source)
            {
                if (predicate(element))
                {
                    itemsForRemoval.Add(element);
                }
            }

            foreach (T element in itemsForRemoval)
            {
                source.Remove(element);
            }
        }
    }
}
