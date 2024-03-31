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
    public class FileRootSetting : BaseSetting, IHurlSetting, INotifyPropertyChanged
    {
        public const string CONFIGURATION_NAME = "file_root";

        private string? _directory;

        public FileRootSetting() : base()
        {
            
        }

        public string? Directory
        {
            get => _directory;
            set
            {
                _directory = value;
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
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.Directory = value;
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

            if (!string.IsNullOrWhiteSpace(_directory))
            {
                arguments.Add(new FileRootArgument(_directory));
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
        /// Returns the configuration name (file_root)
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
            return _directory ?? string.Empty;
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return Path.GetDirectoryName(_directory) ?? string.Empty;
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
