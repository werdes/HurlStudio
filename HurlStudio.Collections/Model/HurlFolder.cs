using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HurlStudio.Collections.Model.Collection
{
    public class HurlFolder
    {
        private List<IHurlSetting> _folderSettings;
        private string _location;


        public HurlFolder()
        {        
            _folderSettings = new List<IHurlSetting>();
        }

        public string Location
        {
            get => _location;
            set => _location = value;
        }

        public List<IHurlSetting> FolderSettings
        {
            get => _folderSettings; 
            set => _folderSettings = value; 
        }

        public override string ToString()
        {
            return $"{Location}";
        }
    }
}
