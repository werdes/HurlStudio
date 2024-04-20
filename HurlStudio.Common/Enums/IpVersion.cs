using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Enums
{
    public enum IpVersion
    {
        Undefined,
        Auto,
        IPv4,
        IPv6
    }

    public static class IpVersionExtensions
    {
        private static Dictionary<IpVersion, string> _localization = new Dictionary<IpVersion, string>()
        {
            { IpVersion.Undefined, "Common.Undefined" },
            { IpVersion.Auto, "Setting.IpVersionSetting.Version.Auto" },
            { IpVersion.IPv4, "Setting.IpVersionSetting.Version.IPv4" },
            { IpVersion.IPv6 , "Setting.IpVersionSetting.Version.IPv6" }
        };

        public static IpVersion[] PossibleValues
        {
            get => Enum.GetValues<IpVersion>();
        }

        public static IpVersion[] DisplayValues
        {
            get => PossibleValues.Where(x => x != IpVersion.Undefined).ToArray();
        }

        public static string GetLocalizationKey(this IpVersion version)
        {
            return _localization[version];
        }

    }
}
