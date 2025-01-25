using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public abstract class BaseSettingControl<T> : ViewModelBasedControl<T> where T : INotifyPropertyChanged
    {
        protected T? _viewModel;
        protected MainWindow _mainWindow;
        protected ILogger _log;
        protected INotificationService _notificationService;

        protected BaseSettingControl(MainWindow mainWindow, ILogger logger, INotificationService notificationService)
        {
            _log = logger;
            _notificationService = notificationService;
            _mainWindow = mainWindow;
        }

        protected override void SetViewModelInstance(T viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }
    }
}