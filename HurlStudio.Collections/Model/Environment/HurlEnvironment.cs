using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.Environment
{
    public class HurlEnvironment
    {
        private string? _name;
        private string _fileLocation;
        private List<IHurlSetting> _settings;

        public HurlEnvironment(string location)
        {
            _fileLocation = location;
            _settings = new List<IHurlSetting>();
        }

        public List<IHurlSetting> Settings
        {
            get => _settings;
            set => _settings = value;
        }

        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        public string FileLocation
        {
            get => _fileLocation;
            set => _fileLocation = value;   
        }
    }
}
