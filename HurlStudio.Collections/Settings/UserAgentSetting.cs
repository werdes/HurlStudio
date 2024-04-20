using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.HurlLib.HurlArgument;

namespace HurlStudio.Collections.Settings
{
    public class UserAgentSetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "user_agent";

        private string? _userAgent;

        public UserAgentSetting()
        {
            
        }

        [HurlSettingDisplayString]
        public string? UserAgent
        {
            get => _userAgent;
            set
            {
                _userAgent = value;
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
            if (string.IsNullOrWhiteSpace(value)) return null;
            
            this.UserAgent = value;
            return this;
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            return new IHurlArgument[]
            {
                new CaCertArgument(this.UserAgent ?? string.Empty)
            };
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
        /// Returns the configuration name (user_agent)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the serialized value of this setting (e.g. the cert file path)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return this.UserAgent ?? string.Empty;
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return this.GetConfigurationValue();
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