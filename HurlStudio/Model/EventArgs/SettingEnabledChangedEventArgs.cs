using HurlStudio.Model.Enums;
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
        private HurlSettingSectionType _settingSectionType;

        public SettingEnabledChangedEventArgs(bool enabled, HurlSettingSectionType settingSectionType)
        {
            _enabled = enabled;
            _settingSectionType = settingSectionType;
        }

        public bool Enabled
        {
            get => _enabled;
        }

        public HurlSettingSectionType SettingSectionType
        {
            get => _settingSectionType;
        }
    }
}
