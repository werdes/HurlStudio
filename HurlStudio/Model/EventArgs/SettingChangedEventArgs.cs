using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class SettingChangedEventArgs : System.EventArgs
    {
        private readonly IHurlSetting _setting;

        public SettingChangedEventArgs(IHurlSetting setting)
        {
            _setting = setting;
        }

        public IHurlSetting Setting
        {
            get => _setting;
        }
    }
}
