using Avalonia;
using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.UiState;
using HurlStudio.Services.Editor;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Controls;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Views;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        private IEditorService _editorService;
        private bool _overrideClose = false;

        /// <summary>
        /// Design time constructor
        /// </summary>
        /// <exception cref="AccessViolationException"></exception>
        public MainWindow()
        {
            if (!Design.IsDesignMode) throw new AccessViolationException($"{nameof(MainWindow)} initialized from design time constructor");

            _log = App.Services.GetRequiredService<ILogger<MainWindow>>();
            _configuration = App.Services.GetRequiredService<IConfiguration>();
            _userSettingsService = App.Services.GetRequiredService<IUserSettingsService>();
            _uiStateService = App.Services.GetRequiredService<IUiStateService>();
            _viewModel = App.Services.GetRequiredService<MainWindowViewModel>();
            _controlBuilder = App.Services.GetRequiredService<ServiceManager<ViewModelBasedControl>>();

            this.InitializeComponent();
        }

        public MainWindow(ILogger<MainWindow> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IUiStateService uiStateService, MainWindowViewModel mainWindowViewModel, ServiceManager<ViewModelBasedControl> controlBuilder, IEditorService editorService)
        {
            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _uiStateService = uiStateService;
            _viewModel = mainWindowViewModel;
            _controlBuilder = controlBuilder;
            _editorService = editorService;

            this.DataContext = _viewModel;

            this.InitializeComponent();
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

                ViewBase<MainViewViewModel>? view = _controlBuilder.Get<MainView>();
                if (view != null)
                {
                    // Bind the window offScreenMargin to the view margin
                    // -> this makes sure the window is displayed properly on full screen
                    var offscreenMarginBinding = this.GetObservable(OffScreenMarginProperty);
                    view.Bind(MarginProperty, offscreenMarginBinding);

                    this.Content = view;

                    // Load ui state
                    UiState? uiState = await _uiStateService.GetUiStateAsync(false);
                    if (uiState != null && uiState.MainWindowPosition.Width > 0 && uiState.MainWindowPosition.Height > 0)
                    {
                        this.Position = new PixelPoint((int)uiState.MainWindowPosition.X, (int)uiState.MainWindowPosition.Y);
                        this.Width = (int)uiState.MainWindowPosition.Width;
                        this.Height = (int)uiState.MainWindowPosition.Height;

                        if (uiState.MainWindowIsMaximized)
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
            catch (Exception ex)
            {
                _log?.LogCritical(ex, nameof(this.On_MainWindow_Initialized));
                await MessageBox.ShowError(ex.Message, Localization.Localization.MessageBox_ErrorTitle);
            }
        }

        /// <summary>
        /// Store user settings and ui state on closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void On_MainWindow_Closed(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                _userSettingsService.StoreUserSettings();

                if (_viewModel.MainViewViewModel.EditorView == null) return;

                RootDock? rootDock = _viewModel.MainViewViewModel.EditorView.Layout?.VisibleDockables?.FirstOrDefault() as RootDock;
                ProportionalDock? windowDock = rootDock?.VisibleDockables?.FirstOrDefault() as ProportionalDock;
                ProportionalDock? collectionExplorerDock = windowDock?.VisibleDockables?.Where(x => x is ProportionalDock).Select(x => x as ProportionalDock).FirstOrDefault();

                _uiStateService.SetCollectionExplorerProportion(collectionExplorerDock?.Proportion);
                _uiStateService.SetFileHistory(_viewModel.MainViewViewModel.EditorView);
                _uiStateService.SetCollectionExplorerState(_viewModel.MainViewViewModel.EditorView);

                _uiStateService.StoreUiState();
            }
            catch (Exception ex)
            {
                _log?.LogCritical(ex, nameof(this.On_MainWindow_Initialized));
                await MessageBox.ShowError(ex.Message, Localization.Localization.MessageBox_ErrorTitle);
            }
        }

        /// <summary>
        /// Set the ui states' main window state before the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            try
            {
                _uiStateService.SetMainWindowState(this);
            }
            catch (Exception ex)
            {
                _log?.LogCritical(ex, nameof(this.On_MainWindow_Initialized));
            }
        }

        /// <summary>
        /// Overridden OnClosing handler -> Override closing for editor shutdown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            if (_overrideClose)
            {
                _overrideClose = false;
                return;
            }
            e.Cancel = true;
            this.CloseWindow();
        }

        /// <summary>
        /// Wait for editor service shutdown
        /// -> ask for document closing on opened documents with changes
        /// </summary>
        private async void CloseWindow()
        {
            if (_viewModel != null &&
                _viewModel.MainViewViewModel.EditorView != null)
            {
                _uiStateService.SetActiveDocument(_viewModel.MainViewViewModel.EditorView);
            }

            if (await _editorService.Shutdown())
            {
                _overrideClose = true;
                Close();
            }
        }
    }
}
