using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Settings
{
    public class TimeoutSetting : BaseSetting, IHurlSetting, INotifyPropertyChanged
    {
        public const string CONFIGURATION_NAME = "timeout";
        public const char VALUE_SEPARATOR = '|';
        private const uint DEFAULT_VALUE = 30;

        private uint? _connectTimeoutSeconds;
        private uint? _maxTimeSeconds;

        public TimeoutSetting() : base()
        {

        }

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
        /// Deserializes the supplied configuration string into this instance
        /// </summary>
        /// <param name="value">configuration string</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromString(string value)
        {
            uint connectTimeoutSeconds = DEFAULT_VALUE;
            uint maxTimeSeconds = DEFAULT_VALUE;

            if (uint.TryParse(value.Split(VALUE_SEPARATOR).Get(0), out connectTimeoutSeconds) &&
                uint.TryParse(value.Split(VALUE_SEPARATOR).Get(1), out maxTimeSeconds))
            {
                _connectTimeoutSeconds = connectTimeoutSeconds;
                _maxTimeSeconds = maxTimeSeconds;
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
        /// Returns the serialized value
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return (_connectTimeoutSeconds ?? DEFAULT_VALUE).ToString();
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
