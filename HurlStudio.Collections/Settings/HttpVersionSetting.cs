using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class HttpVersionSetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "http_version";

        private HttpVersion? _version;

        public HttpVersionSetting()
        {

        }

        [HurlSettingDisplayString]
        public HttpVersion? Version
        {
            get => _version;
            set
            {
                _version = value;
                this.Notify();
            }
        }

        /// <summary>
        /// Deserializes the supplied configuration arguments into this instance
        /// </summary>
        /// <param name="arguments">Configuration arguments</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromArguments(string?[] arguments)
        {

            HttpVersion version;
            if (Enum.TryParse<HttpVersion>(arguments.Get(0), out version))
            {
                _version = version;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            switch (_version)
            {
                case HttpVersion.Http1_0: return new[] { new Http1_0Argument() };
                case HttpVersion.Http1_1: return new[] { new Http1_1Argument() };
                case HttpVersion.Http2: return new[] { new Http2Argument() };
                case HttpVersion.Http3: return new[] { new Http3Argument() };
            }

            return new IHurlArgument[] { };
        }

        /// <summary>
        /// Returns null, since this setting isn't key/value based
        /// </summary>
        /// <returns></returns>
        public override string? GetConfigurationKey()
        {
            return null;
        }

        /// <summary>
        /// Returns the configuration name (http_version)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the list of configuration values
        /// </summary>
        /// <returns></returns>
        public override object[] GetConfigurationValues()
        {
            return [_version?.ToString() ?? string.Empty];
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return _version?.GetLocalizationKey() ?? string.Empty;
        }

        /// <summary>
        /// Returns the inheritance behavior -> Overwrite -> Setting is unique to a file
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.Overwrite;
        }

        /// <summary>
        /// Fills the setting with default values for ui based creation
        /// </summary>
        /// <returns></returns>
        public override IHurlSetting? FillDefault()
        {
            this.Version = HttpVersion.Http2;

            return this;
        }
    }
}
