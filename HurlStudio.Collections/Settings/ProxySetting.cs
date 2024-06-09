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
        /// Deserializes the supplied configuration arguments into this instance
        /// </summary>
        /// <param name="arguments">Configuration arguments</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromArguments(string?[] arguments)
        {
            if (Enum.TryParse<ProxyProtocol>(arguments.Get(0), true, out ProxyProtocol protocol))
            {
                this.Protocol = protocol;
            }
            if (ushort.TryParse(arguments.Get(2), out ushort port))
            {
                this.Port = port;
            }

            this.Host = arguments.Get(1);
            this.User = arguments.Get(3);
            this.Password = arguments.Get(4)?.DecodeBase64() ?? string.Empty;

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
        /// Returns the list of configuration values
        /// </summary>
        /// <returns></returns>
        public override object[] GetConfigurationValues()
        {
            return [_protocol ?? ProxyProtocol.Undefined,
                    _host ?? string.Empty,
                    _port.ToString() ?? string.Empty,
                    _user ?? string.Empty,
                    _password?.EncodeBase64() ?? string.Empty];
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
