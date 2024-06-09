using Avalonia.Controls;
using HurlStudio.Common.Extensions;
using HurlStudio.Extensions;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.ViewModels;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace HurlStudio.UI.Views
{
    public partial class AddSettingView : ViewBase<AddSettingViewViewModel>
    {
        private AddSettingViewViewModel? _viewModel;
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private ControlLocator _controlLocator;

        public AddSettingView(AddSettingViewViewModel viewModel, ILogger<AddSettingView> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IEditorService editorService, ControlLocator controlLocator, INotificationService notificationService)
        {
            _viewModel = viewModel;

            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _editorService = editorService;
            _controlLocator = controlLocator;
            _notificationService = notificationService;

            this.DataContext = _viewModel;
            this.DataTemplates.Add(_controlLocator);

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(AddSettingViewViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Opens the URL from the button's tag in a web browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonUrl_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            if (button.Tag is not Uri uri) return;

            try
            {
                uri.OpenWeb();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Add the Setting to the dialog result and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonAddSetting_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                if (this.Window == null) return;
                if (_viewModel == null) return;
                if (_viewModel.SelectedSetting == null) return;

                this.Window.Close(_viewModel.SelectedSetting.SettingInstance);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Filter type list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_TextBoxSearch_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (_viewModel == null) return;
                _viewModel.AvailableSettings.ForEach(x => x.IsVisible = true);

                if (_viewModel.Query == null) return;

                _viewModel.AvailableSettings
                    .Where(x => !_viewModel.Query.IsContainedInAnyNormalized(
                        x.SettingInstance.GetType().Name,
                        x.SettingInstance.GetLocalizedTitle(),
                        x.SettingDocumentation?.LocalizedDescription,
                        x.SettingDocumentation?.LocalizedShortDescription,
                        x.SettingDocumentation?.Description,
                        x.SettingDocumentation?.ShortDescription))
                    .ForEach(x => x.IsVisible = false);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// On View loaded
        /// -> focus search textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_AddSettingView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                this.TextBoxSearch.Focus();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }
    }
}