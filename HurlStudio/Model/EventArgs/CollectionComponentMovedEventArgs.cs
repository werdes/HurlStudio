using HurlStudio.Model.CollectionContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class CollectionComponentMovedEventArgs : System.EventArgs
    {
        private CollectionComponentBase _componentSource;
        private CollectionComponentBase _componentTarget;

        public CollectionComponentMovedEventArgs(CollectionComponentBase source, CollectionComponentBase target)
        {
            _componentSource = source;
            _componentTarget = target;
        }

        public CollectionComponentBase Source { get => _componentSource; }
        public CollectionComponentBase Target { get => _componentTarget; }
    }
}
