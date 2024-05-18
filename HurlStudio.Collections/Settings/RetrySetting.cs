using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;

namespace HurlStudio.Collections.Settings
{
    public class RetrySetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "retry";
        private const char VALUE_SEPARATOR = ':';

        private int? _retryCount;
        private uint? _retryInterval;

        public RetrySetting()
        {
            
        }

        public int? RetryCount
        {
            get => _retryCount;
            set
            {
                _retryCount = value;
                this.Notify();
            }
        }

        public uint? RetryInterval
        {
            get => _retryInterval;
            set
            {
                _retryInterval = value;
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

            string[] parts = value.Split(VALUE_SEPARATOR);
            if (parts.Length != 2) return null;

            if (!int.TryParse(parts.Get(0), out int retryCount)) return null;
            if (!uint.TryParse(parts.Get(1), out uint retryInterval)) return null;

            this.RetryCount = retryCount;
            this.RetryInterval = retryInterval;

            return this;
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            List<IHurlArgument> arguments = new List<IHurlArgument>();

            if(_retryCount.HasValue)
            {
                arguments.Add(new RetryArgument(_retryCount.Value));
            }

            if (_retryInterval.HasValue)
            {
                arguments.Add(new RetryIntervalArgument(_retryInterval.Value));
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
        /// Returns the configuration name (retry)
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
            return $"{this.RetryCount}{VALUE_SEPARATOR}{this.RetryInterval}";
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
            this.RetryCount = -1;
            this.RetryInterval = 1000;

            return this;
        }
    }
}