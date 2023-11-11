using HurlStudio.HurlLib.HurlArgument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Settings
{
    public abstract class BaseSetting
    {
        public abstract string GetConfigurationName();
        public abstract string GetConfigurationValue();

        public string GetConfigurationString()
        {
            string settingName = GetConfigurationName();
            string settingValue = GetConfigurationValue();
            return $"{settingName}={settingValue}";
        }

    }
}
