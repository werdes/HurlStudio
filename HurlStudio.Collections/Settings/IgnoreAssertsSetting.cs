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
    public class IgnoreAssertsSetting : BaseSetting, IHurlSetting, INotifyPropertyChanged
    {
        public const string CONFIGURATION_NAME = "ignore_asserts";

        private bool? _ignoreAsserts;

        public IgnoreAssertsSetting() : base()
        {
            
        }

        public bool? IgnoreAsserts
        {
            get => _ignoreAsserts;
            set
            {
                _ignoreAsserts = value;
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
            bool outVal = false;
            if (bool.TryParse(value, out outVal))
            {
                this.IgnoreAsserts = outVal;
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

            if(_ignoreAsserts.HasValue && _ignoreAsserts.Value)
            {
                arguments.Add(new ContinueOnErrorArgument());
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
        /// Returns the configuration name (ignore_asserts)
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
            return _ignoreAsserts?.ToString() ?? false.ToString();
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
