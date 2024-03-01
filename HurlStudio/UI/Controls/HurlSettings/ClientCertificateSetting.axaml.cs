using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class ClientCertificateSetting : ViewModelBasedControl<Collections.Settings.ClientCertificateSetting>
    {
        private Collections.Settings.ClientCertificateSetting? _caCertSetting;
        private MainWindow _mainWindow;
        private ILogger _log;
        private INotificationService _notificationService;

        public ClientCertificateSetting(ILogger<ClientCertificateSetting> logger, INotificationService notificationService, MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _log = logger;
            _notificationService = notificationService;

            InitializeComponent();
        }

        protected override void SetViewModelInstance(Collections.Settings.ClientCertificateSetting viewModel)
        {
            _caCertSetting = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Display file explorer to select a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonOpen_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_mainWindow == null) return;
            if (_caCertSetting == null) return; 
            
            try
            {
                if (_mainWindow.StorageProvider.CanOpen)
                {
                    FilePickerOpenOptions filePickerOpenOptions = new FilePickerOpenOptions();
                    filePickerOpenOptions.AllowMultiple = false;
                    filePickerOpenOptions.Title = Localization.Localization.Setting_CaCertSetting_FilePicker_Title;
                    filePickerOpenOptions.FileTypeFilter = new[] { FilePickerFileTypes.All };

                    IReadOnlyList<IStorageFile> files = await _mainWindow.StorageProvider.OpenFilePickerAsync(filePickerOpenOptions);

                    if (files.Count == 1)
                    {
                        _caCertSetting.Certificate = files.First().Path.AbsolutePath;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_ButtonOpen_Click));
                _notificationService.Notify(ex);
            }
        }
    }
}
