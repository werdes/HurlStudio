using HurlStudio.Collections.Model.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model
{
    public class HurlComponentBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<HurlComponentPropertyChangedEventArgs>? ComponentPropertyChanged;

        protected void Notify([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            this.ComponentPropertyChanged?.Invoke(this, new HurlComponentPropertyChangedEventArgs(this, propertyName));
        }

        /// <summary>
        /// Fire a ComponentPropertyChanged event when a collection is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_GenericCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.ComponentPropertyChanged?.Invoke(this, new HurlComponentPropertyChangedEventArgs(this));
        }

        /// <summary>
        /// Fire a ComponentPropertyChanged event when a collection items' property is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_GenericCollection_CollectionItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.ComponentPropertyChanged?.Invoke(this, new HurlComponentPropertyChangedEventArgs(this));
        }
    }
}
