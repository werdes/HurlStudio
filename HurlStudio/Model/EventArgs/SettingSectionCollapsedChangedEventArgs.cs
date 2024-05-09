using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class SettingSectionCollapsedChangedEventArgs : System.EventArgs
    {
        private readonly bool _collapsed;

        public SettingSectionCollapsedChangedEventArgs(bool collapsed)
        {
            _collapsed = collapsed;
        }

        public bool Collapsed
        {
            get => _collapsed;
        }
    }
}
