using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class RedirectionsSetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "redirections";

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
        /// Deserializes the supplied configuration arguments into this instance
        /// </summary>
        /// <param name="arguments">Configuration arguments</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromArguments(string?[] arguments)
        {
            if (arguments.Length > 0)
            {
                if (bool.TryParse(arguments.Get(0), out bool allowRedirections))
                {
                    this.AllowRedirections = allowRedirections;
                }
                if (bool.TryParse(arguments.Get(1), out bool redirectionsTrusted))
                {
                    this.RedirectionsTrusted = redirectionsTrusted;
                }
                if (uint.TryParse(arguments.Get(2), out uint maxRedirections))
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
                if (_redirectionsTrusted.HasValue && _redirectionsTrusted.Value)
                {
                    arguments.Add(new LocationTrustedArgument());
                }
                else
                {
                    arguments.Add(new LocationArgument());
                }
            }

            if (_maxRedirections.HasValue)
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
        /// Returns the list of configuration values
        /// </summary>
        /// <returns></returns>
        public override object[] GetConfigurationValues()
        {
            return [_allowRedirections ?? false,
                    _redirectionsTrusted ?? false,
                    _maxRedirections ?? 0];
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

        /// <summary>
        /// Fills the setting with default values for ui based creation
        /// </summary>
        /// <returns></returns>
        public override IHurlSetting? FillDefault()
        {
            this.AllowRedirections = true;
            this.MaxRedirections = 1000;
            this.RedirectionsTrusted = true;

            return this;
        }
    }
}
