using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Extensions
{
    public static class ListExtensions
    {
        public static void AddIfNotNull<T>(this List<T> values, T? obj)
        {
            if (obj != null)
            {
                values.Add(obj);
            }
        }

        public static void AddRangeIfNotNull<T>(this List<T> source, IEnumerable<T>? values)
        {
            if (values != null)
            {
                foreach (T value in values)
                {
                    source.Add(value);
                }
            }
        }

        /// <summary>
        /// Syntactic sugar: RemoveAll without predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void RemoveAll<T>(this List<T> collection)
        {
            collection.RemoveAll(x => true);
        }

        /// <summary>
        /// Returns the index of the first item matching the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns>index of the first item found, -1 otherwise</returns>
        public static int IndexOf<T>(this IList<T> collection, Func<T, bool> predicate)
        {
            T? item = collection.FirstOrDefault(predicate);
            if(item != null) return collection.IndexOf(item);
            return -1;
        }
    }
}
