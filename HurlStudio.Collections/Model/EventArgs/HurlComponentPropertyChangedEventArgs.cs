using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.EventArgs
{
    public class HurlComponentPropertyChangedEventArgs : System.EventArgs
    {
        public HurlComponentBase _component;

        public HurlComponentPropertyChangedEventArgs(HurlComponentBase component)
        {
            _component = component;
        }

        public HurlComponentBase Component
        {
            get => _component;
        }
    }
}
