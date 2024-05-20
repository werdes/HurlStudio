using HurlStudio.Common.UI;
using HurlStudio.Model.HurlContainers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlSettings
{
    public class FolderSettingContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private OrderedObservableCollection<HurlSettingContainer> _containers;
        private HurlFolderContainer _collectionFolder;

        public FolderSettingContainer(HurlFolderContainer folder)
        {
            _containers = new OrderedObservableCollection<HurlSettingContainer>();
            _collectionFolder = folder;
        }

        public OrderedObservableCollection<HurlSettingContainer> Containers
        {
            get => _containers;
            set
            {
                _containers = value;
                this.Notify();
            }
        }

        public HurlFolderContainer Folder
        {
            get => _collectionFolder;
            set
            {
                _collectionFolder = value;
                this.Notify();
            }
        }
    }
}
