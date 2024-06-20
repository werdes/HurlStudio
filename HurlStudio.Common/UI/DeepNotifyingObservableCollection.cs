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
    public class DeepNotifyingObservableCollection<T> : ObservableCollection<T>
    {
        public event PropertyChangedEventHandler? CollectionItemPropertyChanged;

        /// <inheritdoc/>
        public DeepNotifyingObservableCollection()
        {

        }

        /// <inheritdoc/>
        public DeepNotifyingObservableCollection(IEnumerable<T> collection) : base(collection)
        {
            foreach (T item in this)
            {
                if (item is INotifyPropertyChanged notifySource)
                {
                    notifySource.PropertyChanged += this.On_Item_PropertyChanged;
                }
            }
        }


        /// <inheritdoc/>
        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            if (item is INotifyPropertyChanged notifySource)
            {
                notifySource.PropertyChanged -= this.On_Item_PropertyChanged;
                notifySource.PropertyChanged += this.On_Item_PropertyChanged;
            }
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            if (this.Count > index)
            {
                T item = this[index];
                if (item is INotifyPropertyChanged notifySource)
                {
                    notifySource.PropertyChanged -= this.On_Item_PropertyChanged;
                }
            }
            base.RemoveItem(index);
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if (item is INotifyPropertyChanged notifySource)
            {
                notifySource.PropertyChanged -= this.On_Item_PropertyChanged;
            }
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            foreach (T item in this)
            {
                if (item is INotifyPropertyChanged notifySource)
                {
                    notifySource.PropertyChanged -= this.On_Item_PropertyChanged;
                }
            }

            base.ClearItems();
        }

        private void On_Item_PropertyChanged(object? sender, PropertyChangedEventArgs e) => this.CollectionItemPropertyChanged?.Invoke(sender, e);
    }
}
