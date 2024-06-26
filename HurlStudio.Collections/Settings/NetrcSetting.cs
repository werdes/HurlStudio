﻿using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Settings
{
    public class NetrcSetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "netrc";

        private bool? _isOptional;
        private bool? _isAutomatic;
        private string? _file;

        public NetrcSetting()
        {

        }

        public bool? IsOptional
        {
            get => _isOptional;
            set
            {
                _isOptional = value;
                this.Notify();
            }
        }

        public bool? IsAutomatic
        {
            get => _isAutomatic;
            set
            {
                _isAutomatic = value;
                this.Notify();
            }
        }

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
        /// Deserializes the supplied configuration arguments into this instance
        /// </summary>
        /// <param name="arguments">Configuration arguments</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromArguments(string?[] arguments)
        {
            if (arguments.Length > 0)
            {
                if (bool.TryParse(arguments.Get(0), out bool isOptional))
                {
                    this.IsOptional = isOptional;
                }
                if (bool.TryParse(arguments.Get(1), out bool isAutomatic))
                {
                    this.IsAutomatic = isAutomatic;
                }

                this.File = arguments.Get(2);
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

            if (this.IsAutomatic.HasValue && this.IsAutomatic.Value)
            {
                arguments.Add(new NetrcArgument());
            }
            else if (!string.IsNullOrEmpty(this.File))
            {
                if (this.IsOptional.HasValue && this.IsOptional.Value)
                {
                    arguments.Add(new NetrcOptionalArgument(this.File));
                }
                else
                {
                    arguments.Add(new NetrcFileArgument(this.File));
                }
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
        /// Returns the configuration name (netrc)
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
            return [_isOptional ?? false,
                    _isAutomatic ?? true,
                    _file ?? string.Empty];
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
            this.File = string.Empty;
            this.IsOptional = false;
            this.IsAutomatic = true;

            return this;
        }
    }
}
