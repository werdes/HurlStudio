﻿using HurlUI.Common.Enums;
using HurlUI.Common.Extensions;
using HurlUI.HurlLib.HurlArgument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Collections.Settings
{
    public class ProxySetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_KEY = "proxy";
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
            set => _protocol = value;
        }


        public string? Password
        {
            get => _password;
            set => _password = value;
        }

        public string? User
        {
            get => _user;
            set => _user = value;
        }

        public string? Host
        {
            get => _host;
            set => _host = value;
        }
        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// Fills from Configuration line
        /// Format: protocol={HTTP|HTTPS},host={host},port={port},user={user},password={password}
        /// Example:
        /// </summary>
        /// <param name="value"></param>
        public IHurlSetting FillFromString(string value)
        {
            string[] parts = value.Split(SUBCONFIGURATION_SEPARATOR);
            foreach (string part in parts)
            {
                string[] keyValuePair = part.Split(SUBCONFIGURATION_KEY_VALUE_SEPARATOR);
                string subConfigKey = keyValuePair[0];
                string? subConfigValue = keyValuePair[1]?.UrlDecode();

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
        public IHurlArgument[] GetArguments()
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
        /// Returns the unique ini key for this setting
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_KEY;
        }

        /// <summary>
        /// Returns the configuration value as string
        /// </summary>
        /// <returns>The configuration value as string</returns>
        public override string GetConfigurationValue()
        {
            string configValue = string.Empty;
            configValue += $"{SUBCONFIGURATION_KEY_PROTOCOL}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.Protocol}";
            configValue += $"{SUBCONFIGURATION_KEY_HOST}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.Host}";
            configValue += $"{SUBCONFIGURATION_KEY_PORT}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.Port}";

            if (!string.IsNullOrEmpty(this.User))
            {
                configValue += $"{SUBCONFIGURATION_KEY_USER}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.User}";
                configValue += $"{SUBCONFIGURATION_KEY_PASSWORD}{SUBCONFIGURATION_KEY_VALUE_SEPARATOR}{this.Password}";
            }

            return configValue;
        }
    }
}
