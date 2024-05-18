using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class ProxySetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "proxy";
        private const string SUBCONFIGURATION_KEY_PROTOCOL = "protocol";
        private const string SUBCONFIGURATION_KEY_HOST = "host";
        private const string SUBCONFIGURATION_KEY_PORT = "port";
        private const string SUBCONFIGURATION_KEY_USER = "user";
        private const string SUBCONFIGURATION_KEY_PASSWORD = "password";
        private const string SUBCONFIGURATION_SEPARATOR = ",";
        private const string SUBCONFIGURATION_KEY_VALUE_SEPARATOR = ":";

        private string? _host;
        private ushort? _port;
        private string? _user;
        private string? _password;
        private ProxyProtocol? _protocol;

        public ProxySetting()
        {

        }

        public ProxyProtocol? Protocol
        {
            get => _protocol;
            set
            {
                _protocol = value;
                this.Notify();
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                this.Notify();
            }
        }

        public string? User
        {
            get => _user;
            set
            {
                _user = value;
                this.Notify();
            }
        }

        [HurlSettingDisplayString]
        public string? Host
        {
            get => _host;
            set
            {
                _host = value;
                this.Notify();
                this.Notify(nameof(this.DisplayString));
            }
        }

        public ushort? Port
        {
            get => _port;
            set
            {
                _port = value;
                this.Notify();
            }
        }


        /// <summary>
        /// Fills from Configuration line
        /// Format: protocol={HTTP|HTTPS},host={host},port={port},user={user},password={password}
        /// Example:
        /// </summary>
        /// <param name="value"></param>
        public override IHurlSetting? FillFromString(string value)
        {
            string[] parts = value.Split(SUBCONFIGURATION_SEPARATOR);
            foreach (string part in parts)
            {
                string[] keyValuePair = part.Split(SUBCONFIGURATION_KEY_VALUE_SEPARATOR);
                string subConfigKey = keyValuePair[0];
                string? subConfigValue = keyValuePair[1]?.DecodeUrl();

                switch (subConfigKey)
                {
                    case SUBCONFIGURATION_KEY_USER: this.User = subConfigValue; break;
                    case SUBCONFIGURATION_KEY_PASSWORD: this.Password = subConfigValue; break;
                    case SUBCONFIGURATION_KEY_HOST: this.Host = subConfigValue; break;
                    case SUBCONFIGURATION_KEY_PORT: this.Port = Convert.ToUInt16(subConfigValue); break;
                    case SUBCONFIGURATION_KEY_PROTOCOL: this.Protocol = Enum.Parse<ProxyProtocol>(subConfigValue ?? ProxyProtocol.Undefined.ToString(), true); break;
                    default: throw new ArgumentException("Unknown sub-config name: " + subConfigKey);
                }
            }

            return this;
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            ProxyArgument proxyArgument = null;

            if (!string.IsNullOrEmpty(this.User))
            {
                proxyArgument = new ProxyArgument(this.Protocol, this.Host, this.Port, this.User, this.Password);
            }
            else
            {
                proxyArgument = new ProxyArgument(this.Protocol, this.Host, this.Port);
            }

            return new IHurlArgument[]
            {
                proxyArgument
            };
        }

        /// <summary>
        /// Returns null, since the proxy setting is not key-based
        /// </summary>
        /// <returns></returns>
        public override string? GetConfigurationKey()
        {
            return null;
        }

        /// <summary>
        /// Returns the unique ini key for this setting
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the configuration value as string
        /// </summary>
        /// <returns>The configuration value as string</returns>
        public override string GetConfigurationValue()
        {
            string configValue = string.Empty;
            configValue += $"{SUBCONFIGURATION_KEY_PROTOCOL}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.Protocol}";
            configValue += $"{SUBCONFIGURATION_SEPARATOR}{SUBCONFIGURATION_KEY_HOST}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.Host}";
            configValue += $"{SUBCONFIGURATION_SEPARATOR}{SUBCONFIGURATION_KEY_PORT}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.Port}";

            if (!string.IsNullOrEmpty(this.User))
            {
                configValue += $"{SUBCONFIGURATION_SEPARATOR}{SUBCONFIGURATION_KEY_USER}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.User}";
                configValue += $"{SUBCONFIGURATION_SEPARATOR}{SUBCONFIGURATION_KEY_PASSWORD}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.Password}";
            }

            return configValue;
        }

        /// <summary>
        /// Returns a string to display next to the title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return this.Host ?? string.Empty;
        }

        /// <summary>
        /// Returns the inheritance behavior of this setting: 
        ///  Multiple proxy settings are not allowed, so 
        ///  these settings will overwrite each other
        /// </summary>
        /// <returns>Inheritance behavior of this setting</returns>
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
            this.Host = string.Empty;
            this.Port = 8080;
            this.User = string.Empty;
            this.Password = String.Empty;
            this.Protocol = ProxyProtocol.HTTP;

            return this;
        }
    }
}
