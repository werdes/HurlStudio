using HurlStudio.Collections.Attributes;
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
    public class ConnectToSetting : BaseSetting, IHurlSetting, INotifyPropertyChanged
    {
        public const string CONFIGURATION_NAME = "connect_to";
        private readonly Regex CONNECT_TO_SETTING_REGEX = new Regex("([^:]*):([0-9]*):([^:]*):([0-9]*)", RegexOptions.Compiled);

        private string? _host1;
        private ushort? _port1;
        private string? _host2;
        private ushort? _port2;

        public ConnectToSetting() : base()
        {

        }

        [HurlSettingKey]
        public string? Host1
        {
            get => _host1;
            set
            {
                _host1 = value;
                Notify();
            }
        }

        [HurlSettingKey]
        public ushort? Port1
        {
            get => _port1;
            set
            {
                _port1 = value;
                Notify();
            }
        }

        public string? Host2
        {
            get => _host2;
            set
            {
                _host2 = value;
                Notify();
            }
        }

        public ushort? Port2
        {
            get => _port2;
            set
            {
                _port2 = value;
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
            Match match = CONNECT_TO_SETTING_REGEX.Match(value);
            if (match.Success && match.Groups.Count > 0)
            {
                this.Host1 = match.Groups.Values.Get(1)?.Value;
                this.Host2 = match.Groups.Values.Get(3)?.Value;

                string? port1Value = match.Groups.Values.Get(2)?.Value;
                string? port2Value = match.Groups.Values.Get(4)?.Value;
                ushort port1;
                ushort port2;

                if(!string.IsNullOrEmpty(port1Value) &&
                   !string.IsNullOrEmpty(port2Value) &&
                   ushort.TryParse(port1Value, out port1) &&
                   ushort.TryParse(port2Value, out port2))
                {
                    this.Port1 = port1;
                    this.Port2 = port2;

                    return this;
                }                  

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

            if (!string.IsNullOrWhiteSpace(_host1) &&
                !string.IsNullOrWhiteSpace(_host2) &&
                _port1 != null &&
                _port2 != null)
            {
                arguments.Add(new ConnectToArgument(_host1, _port1.Value, _host2, _port2.Value));
            }

            return arguments.ToArray();
        }

        /// <summary>
        /// Returns the setting key.
        /// > this setting is unique for a host1/port1 combination
        /// </summary>
        /// <returns></returns>
        public override string? GetConfigurationKey()
        {
            return $"{this.Host1}:{this.Port1}";
        }

        /// <summary>
        /// Returns the configuration name (connect_to)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the serialized value, consisting of the certificate and password, joined by a separator (|)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return $"{this.Host1}:{this.Port1}:{this.Host2}:{this.Port2}";
        }

        /// <summary>
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return $"{this.Host1}:{this.Port1}";
        }

        /// <summary>
        /// Returns the inheritance behavior -> UniqueKey -> Setting is unique to a file for a Host1/Port1 combination
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.UniqueKey;
        }
    }
}
