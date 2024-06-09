using HurlStudio.Collections.Attributes;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.HurlLib.HurlArgument;

namespace HurlStudio.Collections.Settings
{
    public class ResolveSetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "resolve";

        private string? _host;
        private ushort? _port;
        private string? _address;

        public ResolveSetting()
        {
        }

        [HurlSettingDisplayString]
        public string? Host
        {
            get => _host;
            set
            {
                _host = value;
                this.Notify();
            }
        }

        [HurlSettingDisplayString]
        public ushort? Port
        {
            get => _port;
            set
            {
                _port = value;
                this.Notify();
            }
        }

        public string? Address
        {
            get => _address;
            set
            {
                _address = value;
                this.Notify();
            }
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            if (string.IsNullOrWhiteSpace(_host)) throw new ArgumentNullException(nameof(this.Host));
            if (string.IsNullOrWhiteSpace(_address)) throw new ArgumentNullException(nameof(this.Address));
            if (!_port.HasValue) throw new ArgumentNullException(nameof(this.Port));

            return new IHurlArgument[]
            {
                new ResolveArgument(_host, _port.Value, _address)
            };
        }

        /// <summary>
        /// Deserializes the supplied configuration arguments into this instance
        /// </summary>
        /// <param name="arguments">Configuration arguments</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromArguments(string?[] arguments)
        {
            if (ushort.TryParse(arguments.Get(1), out ushort port))
            {
                this.Port = port;
            }
            
            this.Host = arguments.Get(0);
            this.Address = arguments.Get(2);

            return this;
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
        /// Returns a string to display next to the setting title
        /// </summary>
        /// <returns></returns>
        public override string GetDisplayString()
        {
            return $"{this.Host}:{this.Port}";
        }

        /// <summary>
        /// Returns the inheritance behavior -> Append -> Multiple instances of this setting can be handled by hurl
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.Append;
        }

        /// <summary>
        /// Returns the configuration name (allow_insecure)
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
            return [_host ?? string.Empty,
                    _port?.ToString() ?? string.Empty,
                    _address ?? string.Empty];
        }

        /// <summary>
        /// Fills the setting with default values for ui based creation
        /// </summary>
        /// <returns></returns>
        public override IHurlSetting? FillDefault()
        {
            this.Host = string.Empty;
            this.Port = 80;
            this.Address = string.Empty;

            return this;
        }
    }
}