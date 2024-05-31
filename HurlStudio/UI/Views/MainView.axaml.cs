using Avalonia.Controls;
using HurlStudio.Model.UiState;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Controls;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace HurlStudio.UI.Views
{
    public partial class MainView : ViewBase<MainViewViewModel>
    {
        private MainViewViewModel? _viewModel;
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private ICollectionService _collectionService;
        private IEnvironmentService _environmentService;
        private IEditorService _editorService;
        private IUiStateService _uiStateService;
        private INotificationService _notificationService;
        private ControlLocator _controlLocator;
        private ViewFrameViewModel _viewFrameViewModel;
        private LayoutFactory _layoutFactory;

        /// <summary>
        /// Design time constructor
        /// </summary>
        public MainView()
        {
            if (!Design.IsDesignMode) throw new AccessViolationException($"{nameof(MainView)} initialized from design time constructor");
            if (App.Services == null) return;

            _log = App.Services.GetRequiredService<ILogger<MainView>>();
            _notificationService = App.Services.GetRequiredService<INotificationService>();
            /*_viewModel = App.Services.GetRequiredService<MainViewViewModel>();
            _viewFrameViewModel = App.Services.GetRequiredService<ViewFrameViewModel>();*/

            this.InitializeComponent();
        }

        public MainView(MainViewViewModel viewModel, ILogger<MainView> logger, IConfiguration configuration, IUserSettingsService userSettingsService, ICollectionService collectionService, IEnvironmentService environmentService, IEditorService editorService, ControlLocator controlLocator, IUiStateService uiStateService, INotificationService notificationService, ViewFrameViewModel viewFrameViewModel, LayoutFactory layoutFactory)
        {
            _viewModel = viewModel;

            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _environmentService = environmentService;
            _collectionService = collectionService;
            _environmentService = environmentService;
            _editorService = editorService;
            _controlLocator = controlLocator;
            _uiStateService = uiStateService;
            _notificationService = notificationService;
            _viewFrameViewModel = viewFrameViewModel;
            _layoutFactory = layoutFactory;

            this.DataContext = _viewModel;
            this.DataTemplates.Add(_controlLocator);

            _notificationService.NotificationAdded += this.On_NotificationService_NotificationAdded;

            this.InitializeComponent();
        }

        /// <summary>
        /// MainView loaded 
        ///  -> Set up the view frame and navigate to an empty loading view
        ///  -> Load collections and environments 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentNullException">if no view model or user settings were provided to the view</exception>
        private async void On_MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                if (_viewModel == null || _viewModel.LoadingView == null || _viewModel.EditorView == null)
                    throw new ArgumentNullException($"No view model was provided to {nameof(MainView)}");

                _viewFrameViewModel.SelectedViewModel = _viewModel.LoadingView;


                UserSettings? userSettings = await _userSettingsService.GetUserSettingsAsync(false);
                UiState? uiState = await _uiStateService.GetUiStateAsync(true);

                if (userSettings == null) throw new ArgumentNullException($"No user settings provided");

                // Load enviroments
                _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.LoadingEnvironments;
                _viewModel.EditorView.Environments = await _environmentService.GetEnvironmentContainersAsync();
                _viewModel.EditorView.ActiveEnvironment = _viewModel.EditorView.Environments.FirstOrDefault(x => x.EnvironmentFileLocation == uiState?.ActiveEnvironmentFile) ?? _viewModel.EditorView.Environments.FirstOrDefault();

                // Load collections and open files from previous session
                _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.LoadingCollections;
                _viewModel.EditorView.Collections = await _collectionService.GetCollectionContainersAsync();

                _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.OpeningPreviousSessionFiles;
                _viewModel.EditorView.Layout = _layoutFactory.CreateLayout();
                await _editorService.LoadInitialUserSettings();
                await _editorService.OpenInitialDocuments();

                _viewModel.InitializationCompleted = true;
                _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.Finished;

                _viewFrameViewModel.SelectedViewModel = _viewModel.EditorView;
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_MainView_Loaded));
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Show notification panel on new notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_NotificationService_NotificationAdded(object? sender, Model.EventArgs.NotificationAddedEventArgs e)
        {
            try
            {
                if (_viewModel == null) throw new ArgumentNullException(nameof(_viewModel));
                _viewModel.NotificationsExpanded = true;
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_NotificationService_NotificationAdded));
            }
        }

        /// <summary>
        /// Toggle the notification list visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonToggleNotificationList_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                if (_viewModel == null) throw new ArgumentNullException(nameof(_viewModel));
                _viewModel.NotificationsExpanded = !_viewModel.NotificationsExpanded;
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonToggleNotificationList_Click));
                _notificationService.Notify(ex);
            }
        }

        protected override void SetViewModelInstance(MainViewViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// On MenuItem "Save" Click -> action depending on current view
        ///  Call editor service to save the current file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItemSave_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                // Save in editor view
                if (_viewFrameViewModel.SelectedViewModel is EditorViewViewModel editorViewViewModel)
                {
                    bool saveResult = await _editorService.SaveCurrentDocument();
                    if (!saveResult)
                    {
                        _notificationService.Notify(Model.Notifications.NotificationType.Error, Localization.Localization.View_Editor_Message_Save_Error, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, nameof(On_MenuItemSave_Click));
                _notificationService.Notify(Model.Notifications.NotificationType.Error, Localization.Localization.View_Editor_Message_Save_Error, string.Empty);
            }
        }

    }
}