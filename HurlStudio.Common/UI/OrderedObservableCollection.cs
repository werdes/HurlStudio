using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.UI
{
    public class OrderedObservableCollection<T> : ObservableCollection<T>
    {
        public OrderedObservableCollection() : base()
        {
        }

        public OrderedObservableCollection(List<T> collection) : base(collection)
        {
        }

        public OrderedObservableCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        public void MoveItemUp(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (this.Contains(item))
            {
                int idx = this.IndexOf(item);
                this.MoveItem(idx, idx - 1);
            }
        }

        public void MoveItemDown(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (this.Contains(item))
            {
                int idx = this.IndexOf(item);
                this.MoveItem(idx, idx + 1);
            }
        }
    }
}
