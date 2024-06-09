using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace HurlStudio.Collections.Settings
{
    public class AwsSigV4Setting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "aws_sig_v4";

        private string? _provider1;
        private string? _provider2;
        private string? _region;
        private string? _service;

        public AwsSigV4Setting()
        {

        }

        [HurlSettingDisplayString]
        public string? Provider1
        {
            get => _provider1;
            set
            {
                _provider1 = value;
                this.Notify();
            }
        }

        [HurlSettingDisplayString]
        public string? Provider2
        {
            get => _provider2;
            set
            {
                _provider2 = value;
                this.Notify();
            }
        }

        [HurlSettingDisplayString]
        public string? Region
        {
            get => _region;
            set
            {
                _region = value;
                this.Notify();
            }
        }

        [HurlSettingDisplayString]
        public string? Service
        {
            get => _service;
            set
            {
                _service = value;
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
            this.Provider1 = arguments.Get(0);
            this.Provider2 = arguments.Get(1);
            this.Region = arguments.Get(2);
            this.Service = arguments.Get(3);

            return this;
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            List<IHurlArgument> arguments = new List<IHurlArgument>
            {
                new AwsSigV4Argument(
                    _provider1 ?? string.Empty,
                    _provider2 ?? string.Empty,
                    _region ?? string.Empty,
                    _service ?? string.Empty)
            };
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
        /// Returns the configuration name (aws_sig_v4)
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
            return [_provider1 ?? string.Empty,
                    _provider2 ?? string.Empty,
                    _region ?? string.Empty,
                    _service ?? string.Empty];
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return $"{_provider1}:{_provider2}:{_region}:{_service}";
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
            this.Provider1 = string.Empty;
            this.Provider2 = string.Empty;
            this.Region = string.Empty;
            this.Service = string.Empty;

            return this;
        }
    }
}
