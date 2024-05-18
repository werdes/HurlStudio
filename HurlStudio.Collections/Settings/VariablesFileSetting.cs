﻿using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class VariablesFileSetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "variables_file";

        private string? _file;

        public VariablesFileSetting()
        {
        }

        [HurlSettingDisplayString]
        public string? File
        {
            get => _file;
            set
            {
                _file = value;
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
            if(!string.IsNullOrWhiteSpace(value))
            {
                this.File = value;
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
            return new[]
            {
                new VariablesFileArgument(this.File ?? string.Empty)
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
        /// Returns the configuration name (variables_file)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the serialized value of this setting (e.g. the variable file path)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return this.File ?? string.Empty;
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return Path.GetFileName(this.File) ?? string.Empty;
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
            this.File = string.Empty;

            return this;
        }
    }
}