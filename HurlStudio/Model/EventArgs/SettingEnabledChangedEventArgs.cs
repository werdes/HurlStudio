using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class SettingEnabledChangedEventArgs : System.EventArgs
    {
        private readonly bool _enabled;

        public SettingEnabledChangedEventArgs(bool enabled)
        {
            _enabled = enabled;
        }

        public bool Enabled
        {
            get => _enabled;
        }
    }
}
