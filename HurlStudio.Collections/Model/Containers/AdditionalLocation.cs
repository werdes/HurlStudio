using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.Containers
{
    public class AdditionalLocation : BaseContainer, INotifyPropertyChanged
    {
        private string _path;
        private HurlCollection _collection;

        public AdditionalLocation(string path, HurlCollection collection)
        {
            _path = path;
            _collection = collection;
        }

        public HurlCollection Collection
        {
            get => _collection;
            set
            {
                _collection = value;
                this.Notify();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                this.Notify();
            }
        }
    }
}
