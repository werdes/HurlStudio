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
    public partial class CookieSetting : BaseSettingControl<Collections.Settings.CookieSetting>
    {
        public CookieSetting(ILogger<CookieSetting> logger, INotificationService notificationService,
            MainWindow mainWindow) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Display file explorer to select a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonOpenReadFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                string? file = await this.OpenFileSingle(
                    Localization.Localization.Setting_CookieSetting_OpenReadFile_FilePicker_Title,
                    new[] { FilePickerFileTypes.All });

                if (!string.IsNullOrEmpty(file))
                {
                    _viewModel.CookieReadFile = file;
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonOpenReadFile_Click));
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Display file explorer to select a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonOpenWriteFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                string? file = await this.OpenFileSingle(
                    Localization.Localization.Setting_CookieSetting_OpenReadFile_FilePicker_Title,
                    new[] { FilePickerFileTypes.All });

                if (!string.IsNullOrEmpty(file))
                {
                    _viewModel.CookieWriteFile = file;
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonOpenReadFile_Click));
                _notificationService.Notify(ex);
            }
        }
    }
}