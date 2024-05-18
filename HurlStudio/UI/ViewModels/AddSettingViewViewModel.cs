using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Collections.Settings;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Model.UiState;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public class AddSettingViewViewModel : ViewModelBase
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private MainWindowViewModel? _mainWindowViewModel;
        private ObservableCollection<HurlSettingTypeContainer> _availableSettings;
        private HurlSettingTypeContainer? _selectedSetting;
        private string? _query;

        public AddSettingViewViewModel(ILogger<EditorViewViewModel> logger, IConfiguration configuration, IUserSettingsService userSettingsService, MainWindowViewModel mainWindowViewModel) : base(typeof(AddSettingView))
        {
            _log = logger;
            _userSettingsService = userSettingsService;
            _mainWindowViewModel = mainWindowViewModel;
            _availableSettings = new ObservableCollection<HurlSettingTypeContainer>();
            _configuration = configuration; 

            this.RegisterAvailableSettings();
        }

        /// <summary>
        /// Builds an instance of each available type and adds it to the available settings list
        /// </summary>
        private void RegisterAvailableSettings()
        {
            Type[] types = IHurlSetting.GetAvailableTypes();

            foreach (Type settingType in types)
            {
                HurlSettingTypeContainer typeContainer = new HurlSettingTypeContainer(settingType, _configuration);
                typeContainer.SettingTypeContainerSelected += this.On_HurlSettingTypeContainer_SettingTypeContainerSelected;
                _availableSettings.Add(typeContainer);
            }
        }

        /// <summary>
        /// On container selection -> set selectedContainer property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_HurlSettingTypeContainer_SettingTypeContainerSelected(object? sender, Model.EventArgs.SettingTypeContainerSelectedEventArgs e)
        {
            if (e.SelectedHurlSettingTypeContainer == null) return;
            if (!_availableSettings.Contains(e.SelectedHurlSettingTypeContainer)) return;

            this.SelectedSetting = e.SelectedHurlSettingTypeContainer;
        }

        public MainWindowViewModel? MainWindow
        {
            get => _mainWindowViewModel;
            set
            {
                _mainWindowViewModel = value;
                this.Notify();
            }
        }

        public ObservableCollection<HurlSettingTypeContainer> AvailableSettings
        {
            get => _availableSettings;
            set
            {
                _availableSettings = value;
                this.Notify();
            }
        }

        public HurlSettingTypeContainer? SelectedSetting
        {
            get => _selectedSetting;
            set
            {
                _selectedSetting = value;
                this.Notify();
            }
        }

        public string? Query
        {
            get => _query;
            set
            {
                _query = value;
                this.Notify();
            }
        }

    }
}
