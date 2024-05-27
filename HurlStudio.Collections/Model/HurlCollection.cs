using HurlStudio.Collections.Model;
using HurlStudio.Collections.Model.Containers;
using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model
{
    public class HurlCollection : HurlComponentBase
    {
        private string _name;
        private ObservableCollection<AdditionalLocation> _additionalLocations;
        private ObservableCollection<IHurlSetting> _collectionSettings;
        private ObservableCollection<HurlFile> _fileSettings;
        private ObservableCollection<HurlFolder> _folderSettings;
        private string _fileLocation;
        private bool _excludeRootDirectory;

        public HurlCollection(string fileLocation)
        {
            _name = string.Empty;
            _additionalLocations = new ObservableCollection<AdditionalLocation>();
            _collectionSettings = new ObservableCollection<IHurlSetting>();
            _fileSettings = new ObservableCollection<HurlFile>();
            _folderSettings = new ObservableCollection<HurlFolder>();
            _fileLocation = fileLocation;
        }

        public string FileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation = value;
                this.Notify();
            }
        }

        public ObservableCollection<HurlFile> FileSettings
        {
            get => _fileSettings;
            set
            {
                _fileSettings = value;
                this.Notify();

                if (_fileSettings != null)
                {
                    _fileSettings.CollectionChanged -= this.On_GenericCollection_CollectionChanged;
                    _fileSettings.CollectionChanged += this.On_GenericCollection_CollectionChanged;
                }
            }
        }

        public ObservableCollection<HurlFolder> FolderSettings
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

        public ObservableCollection<IHurlSetting> CollectionSettings
        {
            get => _collectionSettings;
            set
            {
                _collectionSettings = value;
                this.Notify();

                if (_collectionSettings != null)
                {
                    _collectionSettings.CollectionChanged -= this.On_GenericCollection_CollectionChanged;
                    _collectionSettings.CollectionChanged += this.On_GenericCollection_CollectionChanged;
                }
            }
        }

        public ObservableCollection<AdditionalLocation> AdditionalLocations
        {
            get => _additionalLocations;
            set
            {
                _additionalLocations = value;
                this.Notify();

                if (_collectionSettings != null)
                {
                    _additionalLocations.CollectionChanged -= this.On_GenericCollection_CollectionChanged;
                    _additionalLocations.CollectionChanged += this.On_GenericCollection_CollectionChanged;
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                this.Notify();
            }
        }

        public bool ExcludeRootDirectory
        {
            get => _excludeRootDirectory;
            set
            {
                _excludeRootDirectory = value;
                this.Notify();
            }
        }
        public override string ToString()
        {
            return $"{FileLocation}, {Name}";
        }
    }
}
