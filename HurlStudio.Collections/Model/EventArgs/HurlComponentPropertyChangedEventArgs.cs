using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.EventArgs
{
    public class HurlComponentPropertyChangedEventArgs : System.EventArgs
    {
        private HurlComponentBase _component;
        private string? _propertyName;

        public HurlComponentPropertyChangedEventArgs(HurlComponentBase component, string? propertyName = null)
        {
            _component = component;
            _propertyName = propertyName;
        }

        public HurlComponentBase Component
        {
            get => _component;
        }

        public string? PropertyName
        {
            get => _propertyName;
        }
    }
}
