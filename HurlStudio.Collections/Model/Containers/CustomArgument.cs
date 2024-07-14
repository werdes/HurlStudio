using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.Containers
{
    public class CustomArgument : BaseContainer, INotifyPropertyChanged
    {
        private string _argument;

        public CustomArgument(string argument)
        {
            _argument = argument;
        }

        public string Argument
        {
            get => _argument;
            set
            {
                _argument = value;
                this.Notify();
            }
        }
    }
}
