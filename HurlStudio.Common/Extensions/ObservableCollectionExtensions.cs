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
    }
}
