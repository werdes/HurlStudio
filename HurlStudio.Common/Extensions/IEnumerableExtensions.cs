﻿using HurlStudio.Common.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public static void ForEach<T>(this IList<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }

        public static T? Get<T>(this IEnumerable<T> collection, int index)
        {
            if(collection.Count() > index && index >= 0) return collection.ElementAt(index);
            return default(T);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        public static DeepNotifyingObservableCollection<T> ToDeepNotifyingObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new DeepNotifyingObservableCollection<T>(collection);
        }
    }
}
