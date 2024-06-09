using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class MaxFilesizeSetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "max_filesize";

        private uint? _maxFilesize;

        public MaxFilesizeSetting()
        {

        }

        [HurlSettingDisplayString]
        public uint? MaxFilesize
        {
            get => _maxFilesize;
            set
            {
                _maxFilesize = value;
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
            uint maxFilesize;
            if (uint.TryParse(arguments.Get(0), out maxFilesize))
            {
                this.MaxFilesize = maxFilesize;
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

            if (_maxFilesize.HasValue)
            {
                arguments.Add(new DelayArgument(_maxFilesize.Value));
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
        /// Returns the configuration name (max_filesize)
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
            return [_maxFilesize ?? 0];
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
            this.MaxFilesize = 0;

            return this;
        }
    }
}
