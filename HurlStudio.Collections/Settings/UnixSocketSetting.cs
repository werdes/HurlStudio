using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;

namespace HurlStudio.Collections.Settings
{
    public class UnixSocketSetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "unix_socket";

        private string? _path;

        public UnixSocketSetting()
        {
            
        }

        [HurlSettingDisplayString]
        public string? Path
        {
            get => _path;
            set
            {
                _path = value;
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
            this.Path = arguments.Get(0);
            return this;
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            List<IHurlArgument> arguments = new List<IHurlArgument>();

            if (!string.IsNullOrWhiteSpace(_path))
            {
                arguments.Add(new UnixSocketArgument(_path));
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
        /// Returns the configuration name (unix_socket)
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
            return [_path ?? string.Empty];
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
            this.Path = string.Empty;

            return this;
        }
    }
}