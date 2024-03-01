using HurlStudio.Collections.Settings;
using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Utility
{
    public class IniSettingParser : ISettingParser
    {
        private Lazy<Dictionary<string, Type>> _possibleSettingTypesLazy;
        private Dictionary<string, Type> _possibleSettingTypes => _possibleSettingTypesLazy.Value;

        public IniSettingParser()
        {
            _possibleSettingTypesLazy = new Lazy<Dictionary<string, Type>>(() => this.RegisterSettingTypes());

            //this.RegisterSettingTypes();
        }

        /// <summary>
        /// Registers the implementing types of IHurlSetting for parsing
        /// </summary>
        private Dictionary<string, Type> RegisterSettingTypes()
        {
            Dictionary<string, Type> settingTypes = new Dictionary<string, Type>();
            Type interfaceType = typeof(IHurlSetting);
            List<Type> implementingTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                                                .Where(x => interfaceType.IsAssignableFrom(x))
                                                .Where(x => !x.IsAbstract)
                                                .ToList();

            foreach (Type settingType in implementingTypes)
            {
                IHurlSetting? hurlSetting = (IHurlSetting?)Activator.CreateInstance(settingType);
                if (hurlSetting != null)
                {
                    settingTypes.Add(hurlSetting.GetConfigurationName(), settingType);
                }
            }

            return settingTypes;
        }

        /// <summary>
        /// Returns a list of available setting types
        /// </summary>
        /// <returns></returns>
        public Type[] GetAvailableTypes()
        {
            return _possibleSettingTypes.Values.ToArray();
        }

        /// <summary>
        /// Returns a IHurlSetting object from a configuration string
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Hurl Setting</returns>
        public IHurlSetting? Parse(string value)
        {
            string? settingName = value.Split('=').Get(0);
            string? settingValue = string.Join('=', value.Split('=').Skip(1));
            if (!string.IsNullOrEmpty(settingName))
            {
                IHurlSetting? setting = GetSetting(settingName);
                if (setting != null)
                {
                    return setting.FillFromString(settingValue);
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
            if (settingType != null)
            {
                return Activator.CreateInstance(settingType) as IHurlSetting;
            }
            return null;
        }
    }
}
