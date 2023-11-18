using HurlStudio.Collections.Model.Collection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.CollectionContainer
{
    public class CollectionFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private CollectionFolder _collectionFolder;
        private HurlFile? _file;
        private string _location;

        public CollectionFile(CollectionFolder collectionFolder, string location)
        {
            _collectionFolder = collectionFolder;
            _location = location;
        }

        public CollectionFolder Folder
        {
            get => _collectionFolder;
            set
            {
                _collectionFolder = value;
                Notify();
            }
        }

        public HurlFile? File
        {
            get => _file;
            set
            {
                _file = value;
                Notify();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                Notify();
            }
        }
    }
}
