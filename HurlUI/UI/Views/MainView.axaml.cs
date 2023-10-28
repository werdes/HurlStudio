using Avalonia.Controls;
using HurlUI.UI.Controls;
using HurlUI.UI;
using HurlUI.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HurlUI.UI.Views
{
    public partial class MainView : ViewBase
    {
        private MainViewViewModel _viewModel;
        private ViewFrame? _viewFrame;
        private ILogger _log;
        private IConfiguration _configuration;

        public MainView() : base(typeof(MainViewViewModel))
        {
            InitializeComponent();
        }

        public MainView(MainViewViewModel viewModel, ViewFrame viewFrame, ILogger<MainView> logger, IConfiguration configuration) : base(typeof(MainViewViewModel))
        {
            _viewModel = viewModel;
            _viewFrame = viewFrame;

            _log = logger;
            _configuration = configuration;

            this.DataContext = _viewModel;

            InitializeComponent();
        }

        /// <summary>
        /// MainView loaded -> Set up the view frame and navigate to an empty loading view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private async void On_MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewFrame == null) throw new ArgumentNullException($"No view frame was provided to {nameof(MainView)}");

            try
            {
                this.WindowContent.Content = _viewFrame;
                _viewFrame.NavigateTo(_viewModel.LoadingView);

                _log.LogDebug($"view init");
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_MainView_Loaded));
            }
        }
    }
}