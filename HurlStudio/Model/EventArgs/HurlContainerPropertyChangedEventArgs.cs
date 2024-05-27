using HurlStudio.Collections.Model;
using HurlStudio.Model.HurlContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class HurlContainerPropertyChangedEventArgs : System.EventArgs
    {
        private HurlComponentBase _component;

        public HurlContainerPropertyChangedEventArgs(HurlComponentBase component)
        {
            _component = component;
        }

        public HurlComponentBase Component { get => _component; }
    }
}
