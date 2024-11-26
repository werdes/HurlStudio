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
    public partial class FileRootSetting : BaseSettingControl<Collections.Settings.FileRootSetting>
    {
        public FileRootSetting(ILogger<FileRootSetting> logger, INotificationService notificationService,
            MainWindow mainWindow) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Display file explorer to select a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonOpen_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                string? folder = await this.DisplayOpenDirectoryPickerSingle(
                    Localization.Localization.Setting_FileRootSetting_FolderPicker_Title);

                if (!string.IsNullOrEmpty(folder))
                {
                    _viewModel.Directory = folder;
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonOpen_Click));
                _notificationService.Notify(ex);
            }
        }
    }
}