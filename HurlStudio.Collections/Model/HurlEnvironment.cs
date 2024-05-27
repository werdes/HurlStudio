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
        private string _fileLocation;
        private ObservableCollection<IHurlSetting> _settings;

        public HurlEnvironment(string location)
        {
            _fileLocation = location;
            _settings = new ObservableCollection<IHurlSetting>();
        }

        public ObservableCollection<IHurlSetting> Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                this.Notify();

                if (_settings != null)
                {
                    _settings.CollectionChanged -= this.On_GenericCollection_CollectionChanged;
                    _settings.CollectionChanged += this.On_GenericCollection_CollectionChanged;
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

        public string FileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation = value;
                this.Notify();
            }
        }
    }
}
