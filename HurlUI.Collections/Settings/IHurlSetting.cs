using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Collections.Settings
{
    public interface IHurlSetting
    {
        HurlUI.HurlLib.HurlArgument.IHurlArgument[] GetArguments();
        IHurlSetting FillFromString(string value);
        string GetConfigurationName();
        string GetConfigurationValue();
        string GetConfigurationString();
    }
}
