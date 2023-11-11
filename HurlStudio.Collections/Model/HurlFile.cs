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
            this._fileSettings = new List<IHurlSetting>();
        }

        public string FileLocation
        {
            get => this._fileLocation;
            set => this._fileLocation = value;
        }

        public string FileTitle
        {
            get => this._fileTitle;
            set => this._fileTitle = value;
        }

        public List<IHurlSetting> FileSettings
        {
            get => this._fileSettings; 
            set => this._fileSettings = value; 
        }
    }
}
