using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Windows;
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

            _viewModel.PropertyChanged += On_ViewModel_OnPropertyChanged;
        }

        private void On_ViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {

        }

        protected async Task<string?> OpenFileSingle(string title, FilePickerFileType[] allowedTypes) => await this.OpenFileSingle(_mainWindow.StorageProvider, title, allowedTypes);
        protected async Task<IReadOnlyList<IStorageFile>?> OpenFileMulti(string title, FilePickerFileType[] allowedTypes) => await this.OpenFileMulti(_mainWindow.StorageProvider, title, allowedTypes);
    }
}