using HurlStudio.Collections.Settings;
using HurlStudio.Model.EventArgs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlSettings
{
    public class HurlSettingTypeContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<SettingTypeContainerSelectedEventArgs>? SettingTypeContainerSelected;

        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private IConfiguration _configuration;
        private Type _type;
        private IHurlSetting _settingInstance;
        private string _settingName;
        private bool _isVisible;
        private HurlSettingDocumentation? _settingDocumentation;

        public HurlSettingTypeContainer(Type type, IConfiguration configuration)
        {
            _type = type;
            _isVisible = true;

            this.SetSettingInstance();
            if (_settingInstance == null) throw new ArgumentNullException(nameof(_settingInstance));

            _settingName = _settingInstance.GetConfigurationName();
            _configuration = configuration;

            _settingDocumentation = _configuration.GetSection($"settingDocs:{_settingName}").Get<HurlSettingDocumentation>();
        }

        private void SetSettingInstance()
        {
            IHurlSetting? settingInstance = (IHurlSetting?)Activator.CreateInstance(_type);
            if (settingInstance == null) throw new ArgumentNullException(nameof(settingInstance));

            _settingInstance = settingInstance;
            _settingInstance.IsEnabled = true;
            _settingInstance.FillDefault();
        }

        public Type Type
        {
            get => _type;
        }

        public IHurlSetting SettingInstance
        {
            get => _settingInstance;
        }

        public HurlSettingDocumentation? SettingDocumentation
        {
            get => _settingDocumentation;
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                this.Notify();
            }
        }

        public void Select() => this.SettingTypeContainerSelected?.Invoke(this, new SettingTypeContainerSelectedEventArgs(this));
    }
}
