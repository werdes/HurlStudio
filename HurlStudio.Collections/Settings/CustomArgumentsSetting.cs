using HurlStudio.Collections.Model.Containers;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.HurlLib.HurlArgument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Settings
{
    public class CustomArgumentsSetting : BaseSetting, IHurlSetting
    {
        private const string CONFIGURATION_NAME = "custom_arguments";
        private OrderedObservableCollection<CustomArgument> _arguments;

        public CustomArgumentsSetting()
        {
            _arguments = new OrderedObservableCollection<CustomArgument>();
            _arguments.CollectionChanged += this.On_CollectionProperty_CollectionChanged;
        }

        public OrderedObservableCollection<CustomArgument> Arguments
        {
            get => _arguments;
            set
            {
                _arguments.CollectionChanged -= this.On_CollectionProperty_CollectionChanged;
                _arguments = value;
                this.Notify();

                _arguments.CollectionChanged += this.On_CollectionProperty_CollectionChanged;
            }
        }

        /// <summary>
        /// Returns the Hurl arguments for this setting
        /// </summary>
        /// <returns></returns>
        public override IHurlArgument[] GetArguments()
        {
            List<IHurlArgument> arguments = new List<IHurlArgument>();
            if (_arguments.Count > 0)
            {
                arguments.Add(new NoProxyArgument(_arguments.Select(x => x.Argument).ToList()));
            }

            return arguments.ToArray();
        }

        /// <summary>
        /// Deserializes the supplied configuration arguments into this instance
        /// </summary>
        /// <param name="arguments">Configuration arguments</param>
        /// <returns></returns>
        public override IHurlSetting? FillFromArguments(string?[] arguments)
        {
            _arguments.AddRange(arguments.Select(x => new CustomArgument(x ?? string.Empty)));
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
            return string.Empty;
        }

        /// <summary>
        /// Returns the inheritance behavior -> Append
        /// </summary>
        /// <returns></returns>
        public override HurlSettingInheritanceBehavior GetInheritanceBehavior()
        {
            return HurlSettingInheritanceBehavior.Append;
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
        /// Returns the list of configuration values
        /// </summary>
        /// <returns></returns>
        public override object[] GetConfigurationValues()
        {
            return _arguments.Select(x => x.Argument).ToArray();
        }

        /// <summary>
        /// Fills the setting with default values for ui based creation
        /// </summary>
        /// <returns></returns>
        public override IHurlSetting? FillDefault()
        {
            this.Arguments.Add(new CustomArgument(string.Empty));

            return this;
        }
    }
}
