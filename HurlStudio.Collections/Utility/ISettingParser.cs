using HurlStudio.Collections.Settings;

namespace HurlStudio.Collections.Utility
{
    public interface ISettingParser
    {
        IHurlSetting? Parse(string value);
    }
}