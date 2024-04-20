using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Enums
{
    public enum HttpVersion
    {
        Undefined,
        Http1_0,
        Http1_1,
        Http2,
        Http3,
    }


    public static class HttpVersionExtensions
    {
        private static Dictionary<HttpVersion, string> _localizations = new Dictionary<HttpVersion, string>()
        {
            { HttpVersion.Undefined, "Common.Undefined" },
            { HttpVersion.Http1_0, "Setting.HttpVersionSetting.Version.Http10" },
            { HttpVersion.Http1_1, "Setting.HttpVersionSetting.Version.Http11" },
            { HttpVersion.Http2, "Setting.HttpVersionSetting.Version.Http20" },
            { HttpVersion.Http3, "Setting.HttpVersionSetting.Version.Http30" }
        };


        public static HttpVersion[] PossibleValues
        {
            get => Enum.GetValues<HttpVersion>();
        }

        public static HttpVersion[] DisplayValues
        {
            get => PossibleValues.Where(x => x != HttpVersion.Undefined).ToArray();
        }

        public static string GetLocalizationKey(this HttpVersion version)
        {
            return _localizations[version];
        }
    }
}
