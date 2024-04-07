using HurlStudio.Common.Enums;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HurlStudio.Collections.Settings
{
    public abstract class BaseSetting : INotifyPropertyChanged, IHurlSetting
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private const string NAME_VALUE_SEPARATOR = "=";

        public BaseSetting()
        {
        }

        public abstract string GetConfigurationName();
        public abstract string GetConfigurationValue();

        public string GetConfigurationString()
        {
            string settingName = this.GetConfigurationName();
            string settingValue = this.GetConfigurationValue();
            return $"{settingName}{NAME_VALUE_SEPARATOR}{settingValue}";
        }

        public abstract IHurlArgument[] GetArguments();
        public abstract IHurlSetting? FillFromString(string value);
        public abstract string? GetConfigurationKey();
        public abstract string GetDisplayString();
        public abstract HurlSettingInheritanceBehavior GetInheritanceBehavior();

        /// <summary>
        /// DisplayString for binding purposes
        /// </summary>
        public string DisplayString
        {
            get => this.GetDisplayString();
        }
    }
}
