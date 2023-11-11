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
        private string _name;
        private List<IHurlSetting> _collectionSettings;

        public HurlEnvironment()
        {
            _name = string.Empty;
            _collectionSettings = new List<IHurlSetting>();
        }

        public List<IHurlSetting> CollectionSettings
        {
            get => _collectionSettings;
            set => _collectionSettings = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
}
