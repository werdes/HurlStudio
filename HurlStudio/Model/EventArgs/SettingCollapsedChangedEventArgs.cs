using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class SettingCollapsedChangedEventArgs : System.EventArgs
    {
        private readonly bool _collapsed;

        public SettingCollapsedChangedEventArgs(bool collapsed)
        {
            _collapsed = collapsed;
        }

        public bool Collapsed
        {
            get => _collapsed;
        }
    }
}
