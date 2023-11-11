using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HurlStudio.Collections.Model.Collection
{
    public class HurlFile
    {
        private List<IHurlSetting> _fileSettings;
        private string _fileLocation;
        private string _fileTitle;


        public HurlFile()
        {
            _fileSettings = new List<IHurlSetting>();
        }

        public string FileLocation
        {
            get => _fileLocation;
            set => _fileLocation = value;
        }

        public string FileTitle
        {
            get => _fileTitle;
            set => _fileTitle = value;
        }

        public List<IHurlSetting> FileSettings
        {
            get => _fileSettings; 
            set => _fileSettings = value; 
        }
    }
}
