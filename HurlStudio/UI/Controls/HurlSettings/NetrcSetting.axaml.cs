using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class NetrcSetting : BaseSettingControl<Collections.Settings.NetrcSetting>
    {
        public NetrcSetting(MainWindow mainWindow, ILogger<NetrcSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// Display file explorer to select a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonOpenFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                string? netrcFile = await this.DisplayOpenFilePickerSingle(
                    Localization.Localization.Setting_NetrcSetting_File_FilePicker_Title,
                    new[] { FilePickerFileTypes.All });
                if (!string.IsNullOrEmpty(netrcFile))
                {
                    _viewModel.File = netrcFile;
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonOpenFile_Click));
                _notificationService.Notify(ex);
            }
        }

    }
}