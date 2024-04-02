using System.ComponentModel;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.HurlLib.HurlArgument;

namespace HurlStudio.Collections.Settings
{
    public class NoProxySetting : BaseSetting, IHurlSetting, INotifyPropertyChanged
    {
        private const string CONFIGURATION_NAME = "file_root";
        private const string VALUE_SEPARATOR = ",";
        
        private OrderedObservableCollection<string> _noProxyHosts;

        public NoProxySetting() : base()
        {
            _noProxyHosts = new OrderedObservableCollection<string>();
        }

        public OrderedObservableCollection<string> NoProxyHosts
        {
            get => _noProxyHosts;
            set
            {
                _noProxyHosts = value;
                this.Notify();
            }
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            List<IHurlArgument> arguments = new List<IHurlArgument>();
            if (_noProxyHosts.Count > 0)
            {
                arguments.Add(new NoProxyArgument(_noProxyHosts));
            }

            return arguments.ToArray();
        }

        /// <summary>
        /// Deserializes the supplied configuration string into this instance
        /// </summary>
        /// <param name="value">configuration string</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromString(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            
            _noProxyHosts.AddRange(value.Split(VALUE_SEPARATOR));
            return _noProxyHosts.Count > 0 ? this : null;
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
        /// Returns the configuration name (file_root)
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationName()
        {
            return CONFIGURATION_NAME;
        }

        /// <summary>
        /// Returns the serialized value or "false", if null
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationValue()
        {
            return string.Join(VALUE_SEPARATOR, _noProxyHosts);
        }
    }
}