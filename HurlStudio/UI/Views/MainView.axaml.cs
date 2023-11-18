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

namespace HurlStudio.UI.Views
{
    public partial class MainView : ViewBase
    {
        private MainViewViewModel _viewModel;
        private ViewFrame? _viewFrame;
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private ICollectionService _collectionService;
        private IEnvironmentService _environmentService;

        public MainView() : base(typeof(MainViewViewModel))
        {
            InitializeComponent();
        }

        public MainView(MainViewViewModel viewModel, ViewFrame viewFrame, ILogger<MainView> logger, IConfiguration configuration, IUserSettingsService userSettingsService, ICollectionService collectionService, IEnvironmentService environmentService) : base(typeof(MainViewViewModel))
        {
            _viewModel = viewModel;
            _viewFrame = viewFrame;

            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _collectionService = collectionService;
            _environmentService = environmentService;

            this.DataContext = _viewModel;

            InitializeComponent();
            _environmentService = environmentService;
        }

        /// <summary>
        /// MainView loaded 
        ///  -> Set up the view frame and navigate to an empty loading view
        ///  -> Load collections and environments 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private async void On_MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                if (_viewFrame == null) throw new ArgumentNullException($"No view frame was provided to {nameof(MainView)}");

                this.WindowContent.Content = _viewFrame;

                if (_viewModel != null &&
                    _viewModel.LoadingView != null &&
                    _viewModel.EditorView != null)
                {
                    _viewFrame.NavigateTo(_viewModel.LoadingView);

                    UserSettings? userSettings = await _userSettingsService.GetUserSettingsAsync(false);
                    _log.LogInformation($"view init");
                    _log.LogDebug(userSettings?.UiLanguageString);

                    _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.LoadingCollections;
                    _viewModel.EditorView.Collections = await _collectionService.GetCollectionContainersAsync();

                    _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.LoadingEnvironments;
                    _viewModel.EditorView.Environments = new ObservableCollection<HurlEnvironment>(await _environmentService.GetEnvironmentsAsync());

                    _viewModel.InitializationCompleted = true;
                    _viewModel.LoadingView.CurrentActivity = Model.Enums.LoadingViewStep.Finished;

                    _viewFrame.NavigateTo(_viewModel.EditorView);
                }
                else throw new ArgumentNullException($"View models were not initialized correctly");
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_MainView_Loaded));
                await this.ShowErrorMessage(ex);
            }
        }
    }
}