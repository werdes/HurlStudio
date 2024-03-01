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
    public class ClientCertificateSetting : BaseSetting, IHurlSetting, INotifyPropertyChanged
    {
        public const string CONFIGURATION_NAME = "client_certificate";
        private const string VALUE_SEPARATOR = "|";
        private readonly Regex CLIENT_CERTIFICATE_SETTING_REGEX = new Regex("([^" + Regex.Escape(VALUE_SEPARATOR) + "]*)" + Regex.Escape(VALUE_SEPARATOR) + "([^" + Regex.Escape(VALUE_SEPARATOR) + "]*)", RegexOptions.Compiled);

        private string? _certificate;
        private string? _password;

        public ClientCertificateSetting() : base()
        {
            
        }

        public string? Certificate
        {
            get => _certificate;
            set
            {
                _certificate = value;
                Notify();
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
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
            Match match = CLIENT_CERTIFICATE_SETTING_REGEX.Match(value);
            if (match.Success && match.Groups.Count > 0)
            {
                this.Certificate = match.Groups.Values.Get(1)?.Value;
                this.Password = match.Groups.Values.Get(2)?.Value;

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
            if (!string.IsNullOrEmpty(this.Certificate))
            {
                if (!string.IsNullOrEmpty(this.Password))
                {
                    arguments.Add(new ClientCertificateArgument(this.Certificate, this.Password));
                }
                else
                {
                    arguments.Add(new ClientCertificateArgument(this.Certificate));
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
        /// Returns the configuration name (client_certificate)
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
            return $"{this.Certificate}{VALUE_SEPARATOR}{this.Password}";
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
    }
}
