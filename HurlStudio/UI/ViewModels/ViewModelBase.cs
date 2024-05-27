using Avalonia.Controls;
using HurlStudio.UI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Type _attachedViewType;
        private ViewModelBase? _rootViewModel;
        private bool _active;
        private bool _initializationCompleted;


        public Type AttachedViewType { get => _attachedViewType; }

        public ViewModelBase? RootViewModel
        {
            get => _rootViewModel;
        }

        public ViewModelBase SetRoot(ViewModelBase? rootViewModel)
        {
            _rootViewModel = rootViewModel;
            this.Notify(nameof(this.RootViewModel));

            return this;
        }

        public ViewModelBase(Type attachedViewType)
        {
            _initializationCompleted = false;
            _attachedViewType = attachedViewType;
            _rootViewModel = null;
        }

        public bool IsActive
        {
            get => _active;
            set
            {
                _active = value;
                this.Notify();
            }
        }

        public bool InitializationCompleted
        {
            get => _initializationCompleted;
            set
            {
                _initializationCompleted = value;
                this.Notify();
            }
        }
    }
}
