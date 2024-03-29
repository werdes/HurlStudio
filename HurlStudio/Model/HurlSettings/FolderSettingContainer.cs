﻿using HurlStudio.Model.CollectionContainer;
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
        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private ObservableCollection<HurlSettingContainer> _containers;
        private CollectionFolder _collectionFolder;

        public FolderSettingContainer(CollectionFolder folder)
        {
            _containers = new ObservableCollection<HurlSettingContainer>();
            _collectionFolder = folder;
        }

        public ObservableCollection<HurlSettingContainer> Containers
        {
            get => _containers;
            set
            {
                _containers = value;
                Notify();
            }
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
    }
}
