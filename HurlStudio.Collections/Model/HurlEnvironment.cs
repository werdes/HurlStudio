using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model
{
    public class HurlEnvironment : HurlComponentBase
    {
        private string? _name;
        private string _environmentFileLocation;
        private ObservableCollection<IHurlSetting> _environmentSettings;

        public HurlEnvironment(string location)
        {
            _environmentFileLocation = location;
            _environmentSettings = new ObservableCollection<IHurlSetting>();
        }

        public ObservableCollection<IHurlSetting> EnvironmentSettings
        {
            get => _environmentSettings;
            set
            {
                _environmentSettings = value;
                this.Notify();

                if (_environmentSettings != null)
                {
                    _environmentSettings.CollectionChanged -= this.On_GenericCollection_CollectionChanged;
                    _environmentSettings.CollectionChanged += this.On_GenericCollection_CollectionChanged;
                }
            }
        }

        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                this.Notify();
            }
        }

        public string EnvironmentFileLocation
        {
            get => _environmentFileLocation;
            set
            {
                _environmentFileLocation = value;
                this.Notify();
            }
        }
    }
}
