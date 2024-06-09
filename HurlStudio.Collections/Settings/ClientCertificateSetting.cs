using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;

namespace HurlStudio.Collections.Settings
{
    public class ClientCertificateSetting : BaseSetting, IHurlSetting
    {
        public const string CONFIGURATION_NAME = "client_certificate";
        private const string VALUE_SEPARATOR = ",";

        private string? _certificateFile;
        private string? _password;
        private string? _keyFile;

        public ClientCertificateSetting()
        {
            
        }

        [HurlSettingDisplayString]
        public string? CertificateFile
        {
            get => _certificateFile;
            set
            {
                _certificateFile = value;
                this.Notify();
            }
        }

        [HurlSettingDisplayString]
        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                this.Notify();
            }
        }

        public string? KeyFile
        {
            get => _keyFile;
            set
            {
                _keyFile = value;
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
                this.CertificateFile = arguments.Get(0);
                this.KeyFile = arguments.Get(2);

                string? passwordBase64 = arguments.Get(1);
                if (passwordBase64 != null)
                {
                    this.Password = passwordBase64.DecodeBase64();
                }

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

            if (!string.IsNullOrEmpty(this.CertificateFile))
            {
                if (!string.IsNullOrEmpty(this.Password))
                {
                    arguments.Add(new ClientCertificateArgument(this.CertificateFile, this.Password));
                }
                else
                {
                    arguments.Add(new ClientCertificateArgument(this.CertificateFile));
                }
            }
            if (!string.IsNullOrEmpty(this.KeyFile))
            {
                arguments.Add(new KeyArgument(this.KeyFile));
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
        /// Returns the list of configuration values
        /// </summary>
        /// <returns></returns>
        public override object[] GetConfigurationValues()
        {
            return [_certificateFile ?? string.Empty,
                    _password?.EncodeBase64() ?? string.Empty,
                    _keyFile ?? string.Empty];
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
            this.KeyFile = string.Empty;
            this.CertificateFile = string.Empty;
            this.Password = string.Empty;

            return this;
        }
    }
}
