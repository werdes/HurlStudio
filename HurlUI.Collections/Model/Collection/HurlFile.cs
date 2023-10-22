using HurlUI.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HurlUI.Collections.Model.Collection
{
    public class HurlFile
    {
        private List<IHurlSetting> _fileSettings;
        private string _fileLocation;


        public HurlFile()
        {        
            _fileSettings = new List<IHurlSetting>();
        }

        public string FileLocation
        {
            get => _fileLocation;
            set => _fileLocation = value;
        }

        public List<IHurlSetting> FileSettings
        {
            get => _fileSettings; 
            set => _fileSettings = value; 
        }
    }
}
