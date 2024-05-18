using HurlStudio.Collections.Model.EventArgs;
using HurlStudio.Common.Enums;
using HurlStudio.HurlLib.HurlArgument;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace HurlStudio.Collections.Settings
{
    public abstract class BaseSetting : INotifyPropertyChanged, IHurlSetting
    {
        private const string NAME_VALUE_SEPARATOR = "=";
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<SettingPropertyChangedEventArgs>? SettingPropertyChanged;

        private bool _isEnabled;

        protected void Notify([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if(propertyName != nameof(this.DisplayString))
            {
                this.SettingPropertyChanged?.Invoke(this, new SettingPropertyChangedEventArgs(this));
            }
        }

        public BaseSetting()
        {
        }

        [JsonIgnore]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                this.Notify();
            }
        }

        public abstract string GetConfigurationName();
        public abstract string GetConfigurationValue();

        public string GetConfigurationString()
        {
            string active = this.IsEnabled ? string.Empty : "#";
            string settingName = this.GetConfigurationName();
            string settingValue = this.GetConfigurationValue();
            return $"{active}{settingName}{NAME_VALUE_SEPARATOR}{settingValue}";
        }

        public abstract IHurlArgument[] GetArguments();
        public abstract IHurlSetting? FillFromString(string value);
        public abstract IHurlSetting? FillDefault();
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

        /// <summary>
        /// Raises the NotifyPropertyChanged for the display string
        /// </summary>
        public void RefreshDisplayString()
        {
            this.Notify(nameof(DisplayString));
        }

        /// <summary>
        /// Propagate a collection change event to notify that something in this setting has changed via SettingPropertyChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_CollectionProperty_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.SettingPropertyChanged?.Invoke(this, new SettingPropertyChangedEventArgs(this));

            List<object>? oldNotifiableItems = e.OldItems?.Cast<object>().Where(x => x is INotifyPropertyChanged).ToList();
            List<object>? newNotifiableItems = e.NewItems?.Cast<object>().Where(x => x is INotifyPropertyChanged).ToList();

            // Unsubscribe from the removed items' PropertyChanged event for propagation to SettingPropertyChanged
            oldNotifiableItems?.ForEach(x => ((INotifyPropertyChanged)x).PropertyChanged -= On_CollectionProperty_Item_PropertyChanged);
            // Subscribe to the added items' PropertyChanged event for propagation to SettingPropertyChanged
            newNotifiableItems?.ForEach(x => ((INotifyPropertyChanged)x).PropertyChanged += On_CollectionProperty_Item_PropertyChanged);
        }

        /// <summary>
        /// Propagate a collection items' property change event to notify that something in this setting has changed via SettingPropertyChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_CollectionProperty_Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.SettingPropertyChanged?.Invoke(this, new SettingPropertyChangedEventArgs(this));
        }
    }
}
