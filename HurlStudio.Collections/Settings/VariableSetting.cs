using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.Text.RegularExpressions;

namespace HurlStudio.Collections.Settings
{
    public class VariableSetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "variable";

        private string? _key;
        private string? _value;

        public VariableSetting()
        {

        }

        [HurlSettingDisplayString]
        [HurlSettingKey]
        public string? Key
        {
            get => _key;
            set
            {
                _key = value;
                this.Notify();
                this.Notify(nameof(this.DisplayString));
            }
        }

        public string? Value
        {
            get => _value;
            set
            {
                _value = value;
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
            this.Key = arguments.Get(0);
            this.Value = arguments.Get(1);

            return this;
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            List<IHurlArgument> arguments = new List<IHurlArgument>();
            if (this.Key != null)
            {
                arguments.Add(new VariableArgument(this.Key, this.Value ?? string.Empty));
            }
            return arguments.ToArray();
        }

        /// <summary>
        /// Returns the unique key (variable key) for this setting
        /// </summary>
        /// <returns></returns>
        public override string? GetConfigurationKey()
        {
            return this.Key;
        }

        /// <summary>
        /// Returns the configuration name (variable)
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
            return [_key ?? string.Empty,
                    _value ?? string.Empty];
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return this.Key ?? string.Empty;
        }

        /// <summary>
        /// Returns the inheritance behavior -> UniqueKey, so overwritten by key
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.UniqueKey;
        }

        /// <summary>
        /// Fills the setting with default values for ui based creation
        /// </summary>
        /// <returns></returns>
        public override IHurlSetting? FillDefault()
        {
            this.Key = string.Empty;
            this.Value = string.Empty;

            return this;
        }
    }
}
