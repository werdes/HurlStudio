using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.Containers
{
    public class NoProxyHost : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _host;

        public NoProxyHost(string host)
        {
            _host = host;
        }

        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                this.Notify();
            }
        }
    }
}
