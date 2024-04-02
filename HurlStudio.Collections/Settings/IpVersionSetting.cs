using HurlStudio.Collections.Enums;
using HurlStudio.Common.Enums;
using HurlStudio.HurlLib.HurlArgument;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Settings
{
    public class IpVersionSetting : BaseSetting, IHurlSetting, INotifyPropertyChanged
    {
        public const string CONFIGURATION_NAME = "ip_version";

        private IpVersion? _ipVersion;

        public IpVersionSetting() : base()
        {
            
        }

        public IpVersion? IpVersion
        {
            get => _ipVersion;
            set
            {
                _ipVersion = value;
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
            IpVersion ipVersion;
            if (Enum.TryParse<IpVersion>(value, out ipVersion))
            {
                _ipVersion = ipVersion;
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
            switch (_ipVersion)
            {
                case Enums.IpVersion.IPv4: return new[] { new IPv4Argument() };
                case Enums.IpVersion.IPv6: return new[] { new IPv6Argument() };
            }

            // In case "auto" or no value is selected, don't provide an argument and let hurl decide
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
        /// Returns the configuration name (ip_version)
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
            return _ipVersion?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            switch (_ipVersion)
            {
                case Enums.IpVersion.IPv4: return "v4";
                case Enums.IpVersion.IPv6: return "v6";
                case Enums.IpVersion.Auto: return "auto";
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
