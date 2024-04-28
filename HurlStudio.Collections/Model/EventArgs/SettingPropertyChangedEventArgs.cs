using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Model.EventArgs
{
    public class SettingPropertyChangedEventArgs : System.EventArgs
    {
        private readonly IHurlSetting _setting;

        public SettingPropertyChangedEventArgs(IHurlSetting setting)
        {
            _setting = setting;
        }

        public IHurlSetting Setting
        {
            get => _setting;
        }
    }
}
