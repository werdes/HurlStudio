using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.Text.RegularExpressions;

namespace HurlStudio.Collections.Settings
{
    public class ConnectToSetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "connect_to";
        
        private string? _host1;
        private ushort? _port1;
        private string? _host2;
        private ushort? _port2;

        public ConnectToSetting()
        {

        }

        [HurlSettingKey]
        [HurlSettingDisplayString]
        public string? Host1
        {
            get => _host1;
            set
            {
                _host1 = value;
                this.Notify();
            }
        }

        [HurlSettingKey]
        [HurlSettingDisplayString]
        public ushort? Port1
        {
            get => _port1;
            set
            {
                _port1 = value;
                this.Notify();
            }
        }

        public string? Host2
        {
            get => _host2;
            set
            {
                _host2 = value;
                this.Notify();
            }
        }

        public ushort? Port2
        {
            get => _port2;
            set
            {
                _port2 = value;
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
                this.Host1 = arguments.Get(0);
                this.Host2 = arguments.Get(2);

                string? port1Value = arguments.Get(1);
                string? port2Value = arguments.Get(3);
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
        /// Returns the list of configuration values
        /// </summary>
        /// <returns></returns>
        public override object[] GetConfigurationValues()
        {
            return [_host1 ?? string.Empty,
                    _port1.ToString() ?? string.Empty,
                    _host2 ?? string.Empty,
                    _port2.ToString() ?? string.Empty];
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

        /// <summary>
        /// Fills the setting with default values for ui based creation
        /// </summary>
        /// <returns></returns>
        public override IHurlSetting? FillDefault()
        {
            this.Host1 = string.Empty;
            this.Port1 = null;
            this.Host2 = string.Empty;
            this.Port2 = null;

            return this;
        }
    }
}
