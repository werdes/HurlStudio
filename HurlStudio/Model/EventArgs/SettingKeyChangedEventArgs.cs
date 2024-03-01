using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class SettingKeyChangedEventArgs : System.EventArgs
    {
        private readonly HurlSettingContainer _hurlSetting;

        public SettingKeyChangedEventArgs(HurlSettingContainer setting)
        {
            _hurlSetting = setting;
        }

        public HurlSettingContainer Setting { get => _hurlSetting; }
    }
}
