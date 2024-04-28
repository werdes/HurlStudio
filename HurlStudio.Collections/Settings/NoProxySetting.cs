using HurlStudio.Collections.Model.Settings;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.HurlLib.HurlArgument;

namespace HurlStudio.Collections.Settings
{
    public class NoProxySetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "no_proxy";
        private const string VALUE_SEPARATOR = ",";
        
        private OrderedObservableCollection<NoProxyHost> _noProxyHosts;

        public NoProxySetting()
        {
            _noProxyHosts = new OrderedObservableCollection<NoProxyHost>();
            _noProxyHosts.CollectionChanged += this.On_CollectionProperty_CollectionChanged;
        }

        public OrderedObservableCollection<NoProxyHost> NoProxyHosts
        {
            get => _noProxyHosts;
            set
            {
                _noProxyHosts.CollectionChanged -= this.On_CollectionProperty_CollectionChanged;
                _noProxyHosts = value;
                this.Notify();

                _noProxyHosts.CollectionChanged += this.On_CollectionProperty_CollectionChanged;
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
                arguments.Add(new NoProxyArgument(_noProxyHosts.Select(x => x.Host).ToList()));
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
            
            _noProxyHosts.AddRange(value.Split(VALUE_SEPARATOR).Select(x => new NoProxyHost(x)));
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
        /// Returns the inheritance behavior -> Merge -> Items of this settings will be merged
        /// into a new instance of the returned argument at execution time
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.Merge;
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