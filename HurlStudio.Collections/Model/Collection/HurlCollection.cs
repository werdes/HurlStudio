using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.Collection
{
    public class HurlCollection
    {
        private string _name;
        private List<string> _additionalLocations;
        private List<IHurlSetting> _collectionSettings;
        private List<HurlFile> _fileSettings;
        private List<HurlFolder> _folderSettings;
        private string _fileLocation;
        private bool _excludeRootDirectory;

        public HurlCollection(string fileLocation)
        {
            _name = string.Empty;
            _additionalLocations = new List<string>();
            _collectionSettings = new List<IHurlSetting>();
            _fileSettings = new List<HurlFile>();
            _folderSettings = new List<HurlFolder>();
            _fileLocation = fileLocation;
        }

        public string FileLocation
        {
            get => _fileLocation;
            set => _fileLocation = value;
        }

        public List<HurlFile> FileSettings
        {
            get => _fileSettings;
            set => _fileSettings = value;
        }

        public List<HurlFolder> FolderSettings
        {
            get => _folderSettings;
            set => _folderSettings = value;
        }

        public List<IHurlSetting> CollectionSettings
        {
            get => _collectionSettings;
            set => _collectionSettings = value;
        }

        public List<string> AdditionalLocations
        {
            get => _additionalLocations;
            set => _additionalLocations = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public bool ExcludeRootDirectory
        {
            get => _excludeRootDirectory;
            set => _excludeRootDirectory = value;
        }
        public override string ToString()
        {
            return $"{this.FileLocation}, {this.Name}";
        }
    }
}
