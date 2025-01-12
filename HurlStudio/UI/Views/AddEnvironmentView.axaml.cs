using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using HurlStudio.Collections.Settings;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Extensions;
using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.Editor;
using HurlStudio.Services.HurlFileTemplates;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Editor;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HurlStudio.Collections.Model;
using HurlStudio.Collections.Model.EventArgs;

namespace HurlStudio.UI.Views
{
    public partial class AddEnvironmentView : ViewBase<AddEnvironmentViewViewModel>
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private IHurlFileTemplateService _templateService;
        private ServiceManager<Windows.WindowBase> _windowBuilder;

        public AddEnvironmentView(AddEnvironmentViewViewModel viewModel, ILogger<AddEnvironmentView> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IEditorService editorService, ControlLocator controlLocator, INotificationService notificationService, IHurlFileTemplateService templateService, ServiceManager<Windows.WindowBase> windowBuilder) : base(viewModel, controlLocator)
        {
            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _editorService = editorService;
            _notificationService = notificationService;
            _templateService = templateService;
            _windowBuilder = windowBuilder;

            this.InitializeComponent();
        }

        /// <summary>
        /// On View loaded
        /// -> focus search textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_AddEnvironmentView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Re-Set path on name change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_Environment_ComponentPropertyChanged(object? sender, HurlComponentPropertyChangedEventArgs e)
        {
            if(e.PropertyName == null) return;
            if(e.Component is not HurlEnvironment environment) return;

            if (e.PropertyName == nameof(environment.Name))
            {
                Model.UserSettings.UserSettings? userSettings = await _userSettingsService.GetUserSettingsAsync(true);
                string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GlobalConstants.APPLICATION_DIRECTORY_NAME, GlobalConstants.ENVIRONMENTS_DIRECTORY_NAME);
                
                string? environmentFileName = environment.Name?.GetValidFileName();
                environment.EnvironmentFileLocation = Path.Combine(baseDir, environmentFileName + GlobalConstants.ENVIRONMENT_FILE_EXTENSION);
            }
        }

        /// <summary>
        /// Load available template list on initilization and set up editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_AddEnvironmentView_Initialized(object? sender, System.EventArgs e)
        {
            if (_viewModel == null) return;
            if (_window == null) return;

            try
            {
                // Pull template in from window
                if (_window.GetViewModel() is not AddEnvironmentWindowViewModel windowViewModel) return;
                _viewModel.Environment = windowViewModel.Environment;

                if (_viewModel.Environment != null)
                {
                    _viewModel.Environment.ComponentPropertyChanged += this.On_Environment_ComponentPropertyChanged;
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Close the window with the template container attached
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Button_Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_window == null) return;
            if (_viewModel == null) return;
            if (_viewModel.Environment == null) return;

            try
            {
                if (string.IsNullOrWhiteSpace(_viewModel.Environment.Name)) return;
                if (string.IsNullOrWhiteSpace(_viewModel.Environment.EnvironmentFileLocation)) return;

                _window.Close(_viewModel.Environment);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Close the window without attachment -> discard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Button_Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_window == null) return;

            try
            {
                _window.Close(null);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }


        public override void SetWindow(Windows.WindowBase window)
        {
            base.SetWindow(window);
            _controlLocator.Window = window;
        }
    }
}