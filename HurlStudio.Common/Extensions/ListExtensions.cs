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
        public static void AddIfNotNull<T>(this List<T> values, T obj)
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
    }
}
