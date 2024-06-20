using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.HurlContainers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Documents
{
    public abstract class DocumentBase : Document, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
