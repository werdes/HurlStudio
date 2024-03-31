using Avalonia;
using Avalonia.Controls;
using HurlStudio.Model.UiState;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Controls;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Windows
{
    public partial class MainWindow : WindowBase
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IUiStateService _uiStateService;
        private MainWindowViewModel? _viewModel;
        private ServiceManager<ViewModelBasedControl> _controlBuilder;

        public MainWindow()
        {
            if (!Design.IsDesignMode) throw new AccessViolationException($"{nameof(MainWindow)} initialized from design time constructor");

            _log = App.Services.GetRequiredService<ILogger<MainWindow>>();
            _configuration = App.Services.GetRequiredService<IConfiguration>();
            _userSettingsService = App.Services.GetRequiredService<IUserSettingsService>();
            _uiStateService = App.Services.GetRequiredService<IUiStateService>();
            _viewModel = App.Services.GetRequiredService<MainWindowViewModel>(); 
            InitializeComponent();
        }

        public MainWindow(ILogger<MainWindow> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IUiStateService uiStateService, MainWindowViewModel mainWindowViewModel, ServiceManager<ViewModelBasedControl> controlBuilder)
        {
            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _uiStateService = uiStateService;
            _viewModel = mainWindowViewModel;
            _controlBuilder = controlBuilder;

            InitializeComponent();
        }

        /// <summary>
        /// On window initialization -> attach the main view to the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected async void On_MainWindow_Initialized(object sender, EventArgs e)
        {
            try
            {
                if (_controlBuilder == null) throw new ArgumentNullException($"No control locator was supplied to {nameof(MainWindow)}");

                ViewBase<MainViewViewModel>? view = _controlBuilder?.Get<MainView>();
                if (view != null)
                {
                    // Bind the window offScreenMargin to the view margin
                    // -> this makes sure the window is displayed properly on full screen
                    var offscreenMarginBinding = this.GetObservable(OffScreenMarginProperty);
                    view.Bind(MarginProperty, offscreenMarginBinding);

                    this.Content = view;

                    // Load ui state
                    UiState? uiState = await _uiStateService.GetUiStateAsync(false);
                    if(uiState != null && uiState.MainWindowPosition.Width > 0 && uiState.MainWindowPosition.Height > 0)
                    {
                        this.Position = new PixelPoint((int)uiState.MainWindowPosition.X, (int)uiState.MainWindowPosition.Y);
                        this.Width = (int)uiState.MainWindowPosition.Width;
                        this.Height = (int)uiState.MainWindowPosition.Height;

                        if(uiState.MainWindowIsMaximized)
                        {
                            this.WindowState = WindowState.Maximized;
                        }
                    }
                }
                else
                {
                    this.Content = new TextBlock() { Text = $"No entry view found: {typeof(MainView)}" };
                }
            }
            catch(Exception ex)
            {
                _log?.LogCritical(ex, nameof(On_MainWindow_Initialized));
                await base.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Store user settings and ui state on closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_MainWindow_Closed(object? sender, System.EventArgs e)
        {
            try
            {
                if(_userSettingsService != null)
                {
                    _userSettingsService.StoreUserSettings();
                }

                if (_uiStateService != null && _viewModel != null && _viewModel.MainViewViewModel.EditorView != null)
                {
                    _uiStateService.SetCollectionExplorerState(_viewModel.MainViewViewModel.EditorView);
                    _uiStateService.SetFileHistory(_viewModel.MainViewViewModel.EditorView);
                    _uiStateService.StoreUiState();
                }
            }
            catch (Exception ex)
            {
                _log?.LogCritical(ex, nameof(On_MainWindow_Initialized));
                base.ShowErrorMessage(ex).Wait();
            }
        }

        /// <summary>
        /// Set the ui states' main window state before the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void On_MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            try
            {
                if (_uiStateService != null && _viewModel != null && _viewModel.MainViewViewModel.EditorView != null)
                {
                    _uiStateService.SetMainWindowState(this);
                }
            }
            catch (Exception ex)
            {
                _log?.LogCritical(ex, nameof(On_MainWindow_Initialized));
                await base.ShowErrorMessage(ex);
            }
        }
    }
}
