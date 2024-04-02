using HurlStudio.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
