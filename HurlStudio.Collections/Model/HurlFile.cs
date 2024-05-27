using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HurlStudio.Collections.Model
{
    public class HurlFile : HurlComponentBase
    {
        private ObservableCollection<IHurlSetting> _fileSettings;
        private string _fileLocation;
        private string? _fileTitle;

        public HurlFile()
        {
            _fileSettings = new ObservableCollection<IHurlSetting>();

            // Required for deserialization
            _fileLocation = string.Empty;
        }

        public HurlFile(string fileLocation) : this()
        {
            _fileLocation = fileLocation;
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

        public string? FileTitle
        {
            get => _fileTitle;
            set
            {
                _fileTitle = value;
                this.Notify();
            }
        }

        public ObservableCollection<IHurlSetting> FileSettings
        {
            get => _fileSettings;
            set
            {
                _fileSettings = value;
                this.Notify();

                if (_fileSettings != null)
                {
                    _fileSettings.CollectionChanged -= this.On_GenericCollection_CollectionChanged;
                    _fileSettings.CollectionChanged += this.On_GenericCollection_CollectionChanged;
                }
            }
        }

        public override string ToString()
        {
            return $"{this.FileLocation}, {this.FileTitle}";
        }
    }
}
