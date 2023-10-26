using Avalonia.Controls;
using HurlUI.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace HurlUI.UI.Views
{
    public partial class MainView : UserControl
    {
        private MainViewViewModel _viewModel;
        private static readonly Lazy<Logger> _lazyLog = new Lazy<Logger>(() => LogManager.GetCurrentClassLogger());
        private static NLog.Logger _log => _lazyLog.Value;

        public MainView()
        {
            InitializeComponent();
            _viewModel = new MainViewViewModel()
            {
                DummyText = App.Configuration.GetValue<string>("text")
            };

            this.DataContext = _viewModel;
        }

        private async void On_MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

        }

        private async void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                for(int i = 0; i < 10000; i++)
                {
                    _log.Debug("asdopfiuhalsdkfjhalskdfjhalksdjfhalkdsfjhalkdsjfhalkdsfjhalkdsfj");
                }
            });
        }
    }
}