using Avalonia.Controls;
using HurlUI.UI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Type _attachedViewType;
        private ViewModelBase? _rootViewModel;


        public Type AttachedViewType { get => _attachedViewType; }

        public ViewModelBase? RootViewModel
        {
            get => _rootViewModel;
        }

        public ViewModelBase SetRoot(ViewModelBase? rootViewModel)
        {
            _rootViewModel = rootViewModel;
            Notify(nameof(RootViewModel));

            return this;
        }

        public ViewModelBase(Type attachedViewType)
        {
            _attachedViewType = attachedViewType;
            _rootViewModel = null;
        }
    }
}
