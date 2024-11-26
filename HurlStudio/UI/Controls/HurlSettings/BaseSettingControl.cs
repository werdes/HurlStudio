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
        }

        protected async Task<string?> DisplayOpenFilePickerSingle(string title, FilePickerFileType[] allowedTypes) => 
            await this.DisplayOpenFilePickerSingle(_mainWindow.StorageProvider, title, allowedTypes);
        protected async Task<IReadOnlyList<IStorageFile>?> DisplayOpenFilePickerMulti(string title, FilePickerFileType[] allowedTypes) => 
            await this.DisplayOpenFilePickerMulti(_mainWindow.StorageProvider, title, allowedTypes);
        protected async Task<string?> DisplayOpenDirectoryPickerSingle(string title) => 
            await this.DisplayOpenDirectoryPickerSingle(_mainWindow.StorageProvider, title);
        protected async Task<string?> DisplaySaveFilePicker(string title, string defaultExtension, FilePickerFileType[] allowedTypes) =>
            await this.DisplaySaveFilePickerSingle(_mainWindow.StorageProvider, title, defaultExtension, allowedTypes);
    }
}