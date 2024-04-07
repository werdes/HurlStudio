using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class RedirectionsSetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "redirections";
        private const char VALUE_SEPARATOR = ':';

        private bool? _allowRedirections;
        private bool? _redirectionsTrusted;
        private uint? _maxRedirections;

        public RedirectionsSetting()
        {
            
        }

        public bool? AllowRedirections
        {
            get => _allowRedirections;
            set
            {
                _allowRedirections = value;
                this.Notify();
            }
        }

        public bool? RedirectionsTrusted
        {
            get => _redirectionsTrusted;
            set
            {
                _redirectionsTrusted = value;
                this.Notify();
            }
        }

        public uint? MaxRedirections
        {
            get => _maxRedirections;
            set
            {
                _maxRedirections = value;
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
            string[] parts = value.Split(VALUE_SEPARATOR);
            if(parts.Length > 0)
            {
                if(bool.TryParse(parts.Get(0), out bool allowRedirections))
                {
                    this.AllowRedirections = allowRedirections;
                }
                if(bool.TryParse(parts.Get(1), out bool redirectionsTrusted))
                {
                    this.RedirectionsTrusted = redirectionsTrusted;
                }
                if(uint.TryParse(parts.Get(2), out uint maxRedirections))
                {
                    this.MaxRedirections = maxRedirections;
                }

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
            List<IHurlArgument> arguments = new List<IHurlArgument>();

            if (_allowRedirections.HasValue && _allowRedirections.Value)
            {
                if(_redirectionsTrusted.HasValue && _redirectionsTrusted.Value)
                {
                    arguments.Add(new LocationTrustedArgument());
                }
                else
                {
                    arguments.Add(new LocationArgument());
                }
            }

            if(_maxRedirections.HasValue)
            {
                arguments.Add(new MaxRedirectionsArgument(_maxRedirections.Value));
            }

            return arguments.ToArray();
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
        /// Returns the configuration name (allow_redirections)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the serialized value or "false", if null
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return $"{_allowRedirections}{VALUE_SEPARATOR}{_redirectionsTrusted}{VALUE_SEPARATOR}{_maxRedirections}";
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
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
