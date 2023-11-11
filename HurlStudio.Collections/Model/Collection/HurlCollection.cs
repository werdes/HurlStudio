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
        private List<string> _locations;
        private List<IHurlSetting> _collectionSettings;
        private List<HurlFile> _fileSettings;
        private List<HurlFolder> _folderSettings;

        public HurlCollection()
        {
            _name = string.Empty;
            _locations = new List<string>();
            _collectionSettings = new List<IHurlSetting>();
            _fileSettings = new List<HurlFile>();
            _folderSettings = new List<HurlFolder>();
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

        public List<string> Locations
        {
            get => _locations;
            set => _locations = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
}
