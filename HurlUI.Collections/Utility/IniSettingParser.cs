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
        private static readonly Dictionary<string, Type> _possibleSettingTypes = new Dictionary<string, Type>()
        {
            { ProxySetting.CONFIGURATION_NAME, typeof(ProxySetting) }
        };

        public IniSettingParser()
        {
            
        }

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
                IHurlSetting? setting = GetSetting(settingName);
                if (setting != null)
                {
                    return GetSetting(settingName)?.FillFromString(settingValue);
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new instance of the given setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        private IHurlSetting? GetSetting(string settingName)
        {
            Type? settingType = _possibleSettingTypes.FirstOrDefault(x => x.Key.Equals(settingName)).Value;
            if(settingType != null)
            {
                return Activator.CreateInstance(settingType) as IHurlSetting;
            }
            return null;
        }
    }
}
