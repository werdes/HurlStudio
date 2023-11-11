using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> list)
        {
            return list.Select(x => source.Contains(x)).Any(x => x == true);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }

        public static T? Get<T>(this IEnumerable<T> collection, int index)
        {
            if(collection.Count() > index) return collection.ElementAt(index);
            return default(T);
        }
    }
}
