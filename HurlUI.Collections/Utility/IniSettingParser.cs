using HurlUI.Collections.Settings;
using HurlUI.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Collections.Utility
{
    public class IniSettingParser : ISettingParser
    {
        /// <summary>
        /// Returns a IHurlSetting object from a configuration string
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Hurl Setting</returns>
        public IHurlSetting? Parse(string value)
        {
            string? settingName = value.Split('=').Get(0);
            string? settingValue = value.Split('=').Get(1);
            if (!string.IsNullOrEmpty(settingName) && !string.IsNullOrEmpty(settingValue))
            {
                switch (settingName)
                {
                    case ProxySetting.CONFIGURATION_KEY: return new ProxySetting().FillFromString(settingValue);
                }
            }
            return null;
        }
    }
}
