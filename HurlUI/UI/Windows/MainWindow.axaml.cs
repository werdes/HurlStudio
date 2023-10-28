using Avalonia;
using Avalonia.Controls;
using HurlUI.UI.ViewModels;
using HurlUI.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace HurlUI.UI.Windows
{
    public partial class MainWindow : Window
    {
        private ILogger? _log;
        private IConfiguration? _configuration;
        private ServiceManager<ViewBase>? _viewFactory;

        public MainWindow()
        {
            _log = null;
            _configuration = null;
            _viewFactory = null;
            InitializeComponent();
        }

        public MainWindow(ILogger<MainWindow> logger, IConfiguration configuration, ServiceManager<ViewBase> viewFactory)
        {
            _log = logger;
            _configuration = configuration;
            _viewFactory = viewFactory;
            InitializeComponent();
        }

        /// <summary>
        /// On window initialization -> attach the main view to the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected void On_MainWindow_Initialized(object sender, EventArgs e)
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
    }
}
