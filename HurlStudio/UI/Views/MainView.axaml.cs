using Avalonia.Controls;
using HurlStudio.UI.Controls;
using HurlStudio.UI;
using HurlStudio.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HurlStudio.Services.UserSettings;
using HurlStudio.Model.UserSettings;
using System.Globalization;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using HurlStudio.Collections.Utility;
using HurlStudio.Services.Editor;
using System.Collections.ObjectModel;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using Avalonia;
using Avalonia.Styling;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Services.UiState;
using HurlStudio.Model.UiState;
using HurlStudio.Common.Extensions;
using HurlStudio.Services.Notifications;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Linq;
using HurlStudio.UI.ViewModels.Controls;

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

        /// <summary>
        /// Design time constructor
        /// </summary>
        public MainView()
        {
            if (!Design.IsDesignMode) throw new AccessViolationException($"{nameof(MainView)} initialized from design time constructor");
            
            _log = App.Services.GetRequiredService<ILogger<MainView>>();
            _notificationService = App.Services.GetRequiredService<INotificationService>();
            /*_viewModel = App.Services.GetRequiredService<MainViewViewModel>();
            _viewFrameViewModel = App.Services.GetRequiredService<ViewFrameViewModel>();*/

            this.InitializeComponent();
        }

        public MainView(MainViewViewModel viewModel, ILogger<MainView> logger, IConfiguration configuration, IUserSettingsService userSettingsService, ICollectionService collectionService, IEnvironmentService environmentService, IEditorService editorService, ControlLocator controlLocator, IUiStateService uiStateService, INotificationService notificationService, ViewFrameViewModel viewFrameViewModel)
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
        /// <exception cref="ArgumentNullException">if no frame was provided to the view</exception>
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



                _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.LoadingCollections;
                _viewModel.EditorView.Collections = await _collectionService.GetCollectionContainersAsync();
                _viewModel.EditorView.Documents = await _editorService.GetOpenDocuments();
                _viewModel.EditorView.FileHistoryEntries.AddRangeIfNotNull(uiState?.FileHistoryEntries);
                _viewModel.EditorView.ShowWhitespace = userSettings.ShowWhitespace;
                _viewModel.EditorView.ShowEndOfLine = userSettings.ShowEndOfLine;
                _viewModel.EditorView.WordWrap = userSettings.WordWrap;

                _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.LoadingEnvironments;
                _viewModel.EditorView.Environments = new ObservableCollection<CollectionEnvironment>(
                    (await _environmentService.GetEnvironmentsAsync())
                    .Select(x => new CollectionEnvironment(x)));

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
                if(_viewModel == null) throw new ArgumentNullException(nameof(_viewModel));
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
    }
}