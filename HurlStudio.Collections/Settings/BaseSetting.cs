using HurlStudio.Common.Enums;
using HurlStudio.HurlLib.HurlArgument;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Settings
{
    public abstract class BaseSetting : INotifyPropertyChanged, IHurlSetting
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private const string NAME_VALUE_SEPARATOR = "=";


        public BaseSetting()
        {
        }

        public abstract string GetConfigurationName();
        public abstract string GetConfigurationValue();

        public string GetConfigurationString()
        {
            string settingName = GetConfigurationName();
            string settingValue = GetConfigurationValue();
            return $"{settingName}{NAME_VALUE_SEPARATOR}{settingValue}";
        }

        public abstract IHurlArgument[] GetArguments();
        public abstract IHurlSetting? FillFromString(string value);
        public abstract string? GetConfigurationKey();
        public abstract string GetDisplayString();
        public abstract HurlSettingInheritanceBehavior GetInheritanceBehavior();

        /// <summary>
        /// DisplayString for Binding purposes
        /// </summary>
        public string DisplayString
        {
            get => this.GetDisplayString();
        }
    }
}
