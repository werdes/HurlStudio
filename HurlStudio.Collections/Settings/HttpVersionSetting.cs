using HurlStudio.Collections.Enums;
using HurlStudio.Common.Enums;
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
        /// Deserializes the supplied configuration string into this instance
        /// </summary>
        /// <param name="value">configuration string</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromString(string value)
        {
            HttpVersion version;
            if (Enum.TryParse<HttpVersion>(value, out version))
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

            return new IHurlArgument[] {};
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
        /// Returns the serialized value
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return _version?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            switch (_version)
            {
                case HttpVersion.Http1_0: return "1.0";
                case HttpVersion.Http1_1: return "1.1";
                case HttpVersion.Http2: return "2.0";
                case HttpVersion.Http3: return "3.0";
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the inheritance behavior -> Overwrite -> Setting is unique to a file
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.Overwrite;
        }
    }
}
