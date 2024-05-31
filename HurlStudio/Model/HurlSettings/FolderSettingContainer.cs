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

        private OrderedObservableCollection<HurlSettingContainer> _settingContainers;
        private HurlFolderContainer _folderContainer;

        public FolderSettingContainer(HurlFolderContainer folder)
        {
            _settingContainers = new OrderedObservableCollection<HurlSettingContainer>();
            _folderContainer = folder;
        }

        public OrderedObservableCollection<HurlSettingContainer> SettingContainers
        {
            get => _settingContainers;
            set
            {
                _settingContainers = value;
                this.Notify();
            }
        }

        public HurlFolderContainer FolderContainer
        {
            get => _folderContainer;
            set
            {
                _folderContainer = value;
                this.Notify();
            }
        }
    }
}
