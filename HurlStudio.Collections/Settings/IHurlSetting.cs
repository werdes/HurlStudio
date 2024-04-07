using HurlStudio.Common.Enums;

namespace HurlStudio.Collections.Settings
{
    public interface IHurlSetting
    {
        HurlLib.HurlArgument.IHurlArgument[] GetArguments();
        IHurlSetting? FillFromString(string value);
        string GetConfigurationName();
        string? GetConfigurationKey();
        string GetConfigurationValue();
        string GetConfigurationString();
        string GetDisplayString();
        HurlSettingInheritanceBehavior GetInheritanceBehavior();
    }
}
