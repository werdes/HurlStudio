using HurlStudio.Model.HurlContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class HurlContainerMovedEventArgs : System.EventArgs
    {
        private HurlContainerBase _componentSource;
        private HurlContainerBase _componentTarget;

        public HurlContainerMovedEventArgs(HurlContainerBase source, HurlContainerBase target)
        {
            _componentSource = source;
            _componentTarget = target;
        }

        public HurlContainerBase Source { get => _componentSource; }
        public HurlContainerBase Target { get => _componentTarget; }
    }
}
