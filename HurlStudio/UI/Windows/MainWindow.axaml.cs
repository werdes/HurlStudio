using Avalonia;
using Avalonia.Controls;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Windows
{
    public partial class MainWindow : WindowBase
    {
        private ILogger? _log;
        private IConfiguration? _configuration;
        private IUserSettingsService? _userSettingsService;
        private IUiStateService? _uiStateService;
        private ServiceManager<ViewBase>? _viewFactory;

        public MainWindow()
        {
            _log = null;
            _configuration = null;
            _viewFactory = null;
            _userSettingsService = null;
            _uiStateService = null;
            InitializeComponent();
        }

        public MainWindow(ILogger<MainWindow> logger, IConfiguration configuration, ServiceManager<ViewBase> viewFactory, IUserSettingsService userSettingsService, IUiStateService uiStateService)
        {
            _log = logger;
            _configuration = configuration;
            _viewFactory = viewFactory;
            _userSettingsService = userSettingsService;
            _uiStateService = uiStateService;

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
                if (_viewFactory == null) throw new ArgumentNullException($"No viewFactory was supplied to {nameof(MainWindow)}");

                ViewBase? view = _viewFactory?.Get(typeof(MainView));
                if (view != null)
                {
                    // Bind the window offScreenMargin to the view margin
                    // -> this makes sure the window is displayed properly on full screen
                    var offscreenMarginBinding = this.GetObservable(OffScreenMarginProperty);
                    view.Bind(MarginProperty, offscreenMarginBinding);

                    this.Content = view;
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

                if (_uiStateService != null)
                {
                    _uiStateService.StoreUiState();
                }
            }
            catch (Exception ex)
            {
                _log?.LogCritical(ex, nameof(On_MainWindow_Initialized));
                base.ShowErrorMessage(ex).Wait();
            }
        }
    }
}
