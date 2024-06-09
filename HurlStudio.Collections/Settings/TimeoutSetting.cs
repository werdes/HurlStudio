using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class TimeoutSetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "timeout";
        public const char VALUE_SEPARATOR = ':';
        private const uint DEFAULT_VALUE = 30;

        private uint? _connectTimeoutSeconds;
        private uint? _maxTimeSeconds;

        public TimeoutSetting()
        {
        }

        [HurlSettingDisplayString]
        public uint? ConnectTimeoutSeconds
        {
            get => _connectTimeoutSeconds;
            set
            {
                _connectTimeoutSeconds = value;
                this.Notify();
            }
        }

        public uint? MaxTimeSeconds
        {
            get => _maxTimeSeconds;
            set
            {
                _maxTimeSeconds = value;
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
            if (uint.TryParse(arguments.Get(0), out uint connectTimeoutSeconds))
            {
                _connectTimeoutSeconds = connectTimeoutSeconds;
            }
            if (uint.TryParse(arguments.Get(1), out uint maxTimeSeconds))
            {
                _maxTimeSeconds = maxTimeSeconds;
            }

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
                new ConnectTimeoutArgument(_connectTimeoutSeconds ?? DEFAULT_VALUE),
                new MaxTimeArgument(_maxTimeSeconds ?? DEFAULT_VALUE)
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
        /// Returns the configuration name (connect_timeout)
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
            return [_connectTimeoutSeconds ?? DEFAULT_VALUE,
                    _maxTimeSeconds ?? DEFAULT_VALUE];
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

        /// <summary>
        /// Fills the setting with default values for ui based creation
        /// </summary>
        /// <returns></returns>
        public override IHurlSetting? FillDefault()
        {
            this.ConnectTimeoutSeconds = DEFAULT_VALUE;
            this.MaxTimeSeconds = DEFAULT_VALUE;

            return this;
        }
    }
}