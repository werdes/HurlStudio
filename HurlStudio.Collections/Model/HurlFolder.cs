using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HurlStudio.Collections.Model
{
    public class HurlFolder : HurlComponentBase
    {
        private ObservableCollection<IHurlSetting> _folderSettings;
        private string _folderLocation;

        public HurlFolder()
        {
            _folderLocation = string.Empty;
            _folderSettings = new ObservableCollection<IHurlSetting>();
        }

        public HurlFolder(string location) : this()
        {
            _folderLocation = location;
        }

        public string FolderLocation
        {
            get => _folderLocation;
            set
            {
                _folderLocation = value;
                this.Notify();
            }
        }

        public ObservableCollection<IHurlSetting> FolderSettings
        {
            get => _folderSettings;
            set
            {
                _folderSettings = value;
                this.Notify();

                if (_folderSettings != null)
                {
                    _folderSettings.CollectionChanged -= this.On_GenericCollection_CollectionChanged;
                    _folderSettings.CollectionChanged += this.On_GenericCollection_CollectionChanged;
                }
            }
        }

        public override string ToString()
        {
            return $"{this.FolderLocation}";
        }
    }
}
