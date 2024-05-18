using HurlStudio.Common.Enums;
using HurlStudio.HurlLib.HurlArgument;

namespace HurlStudio.Collections.Settings
{
    public interface IHurlSetting
    {
        IHurlArgument[] GetArguments();
        IHurlSetting? FillFromString(string value);
        IHurlSetting? FillDefault();
        string GetConfigurationName();
        string? GetConfigurationKey();
        string GetConfigurationValue();
        string GetConfigurationString();
        string GetDisplayString();
        HurlSettingInheritanceBehavior GetInheritanceBehavior();

        bool IsEnabled { get; set; }

        private static Type[]? _availableTypes { get; set; }
        /// <summary>
        /// Returns a list of types implementing IHurlSetting
        /// </summary>
        /// <returns></returns>
        static Type[] GetAvailableTypes()
        {
            if (_availableTypes == null)
            {
                Type interfaceType = typeof(IHurlSetting);
                _availableTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                                                                         .Where(x => interfaceType.IsAssignableFrom(x))
                                                                         .Where(x => !x.IsAbstract)
                                                                         .ToArray();
            }
            return _availableTypes;
        }
    }
}
