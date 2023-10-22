using HurlUI.Collections.Settings;

namespace HurlUI.Collections.Utility
{
    public interface ISettingParser
    {
        IHurlSetting? Parse(string value);
    }
}