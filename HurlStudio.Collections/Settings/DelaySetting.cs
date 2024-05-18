using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class DelaySetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "delay";

        private uint? _delay;

        public DelaySetting()
        {

        }

        [HurlSettingDisplayString]
        public uint? Delay
        {
            get => _delay;
            set
            {
                _delay = value;
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
            uint outVal;
            if (uint.TryParse(value, out outVal))
            {
                this.Delay = outVal;
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

            if (_delay.HasValue)
            {
                arguments.Add(new DelayArgument(_delay.Value));
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
        /// Returns the configuration name (delay)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the serialized value or "0", if null
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return _delay?.ToString() ?? 0.ToString();
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
            this.Delay = 0u;

            return this;
        }
    }
}
