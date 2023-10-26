using HurlUI.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HurlUI.Collections.Model.Collection
{
    public class HurlFolder
    {
        private List<IHurlSetting> _folderSettings;
        private string _directory;


        public HurlFolder()
        {        
            _folderSettings = new List<IHurlSetting>();
        }

        public string Directory
        {
            get => _directory;
            set => _directory = value;
        }

        public List<IHurlSetting> FolderSettings
        {
            get => _folderSettings; 
            set => _folderSettings = value; 
        }
    }
}
