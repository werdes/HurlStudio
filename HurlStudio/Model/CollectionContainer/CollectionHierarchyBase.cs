using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.CollectionContainer
{
    public abstract class CollectionHierarchyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private ObservableCollection<CollectionFile> _files;
        private ObservableCollection<CollectionFolder> _folders;
        private bool _collapsed;

        protected CollectionHierarchyBase()
        {
            _folders = new ObservableCollection<CollectionFolder>();
            _files = new ObservableCollection<CollectionFile>();
            _collapsed = false;
        }

        public ObservableCollection<CollectionFolder> Folders
        {
            get => _folders;
            set
            {
                _folders = value;
                Notify();
            }
        }

        public ObservableCollection<CollectionFile> Files
        {
            get => _files;
            set
            {
                _files = value;
                Notify();
            }
        }

        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                _collapsed = value;
                Notify();
            }
        }
    }
}
