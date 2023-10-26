using Avalonia.Controls;
using HurlUI.UI.ViewModels;
using NLog;
using System;

namespace HurlUI.UI.Windows
{
    public partial class MainWindow : Window
    {
        private static readonly Lazy<Logger> _lazyLog = new Lazy<Logger>(() => LogManager.GetCurrentClassLogger());
        private static NLog.Logger _log => _lazyLog.Value;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();
        }
    }
}
