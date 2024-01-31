using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Settings
{
    public class VariableSetting : BaseSetting, IHurlSetting, INotifyPropertyChanged
    {
        public const string CONFIGURATION_NAME = "variable";
        private const string KEY_VALUE_SEPARATOR = ":";

        private readonly Regex VARIABLE_SETTING_REGEX = new Regex("([A-Za-z0-9_\\-]+)(?:\\:)(.*)", RegexOptions.Compiled);

        private string? _key;
        private string? _value;

        public VariableSetting() : base()
        {
            
        }

        public string? Key
        {
            get => _key;
            set
            {
                _key = value;
                Notify();
                Notify(nameof(DisplayString));
            }
        }

        public string? Value
        {
            get => _value;
            set
            {
                _value = value;
                Notify();
            }
        }

        /// <summary>
        /// Deserializes the supplied configuration string into this instance
        /// </summary>
        /// <param name="value">configuration string</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromString(string value)
        {
            Match match = VARIABLE_SETTING_REGEX.Match(value);
            if (match.Success && match.Groups.Count > 0)
            {
                this.Key = match.Groups.Values.Get(1)?.Value;
                this.Value = match.Groups.Values.Get(2)?.Value;

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
        /// Returns the serialized value, consisting of the variable key and value
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return this.Key + KEY_VALUE_SEPARATOR + (this.Value ?? string.Empty);
        }

        /// <summary>
        /// Returns a string to display next to the title
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
    }
}
