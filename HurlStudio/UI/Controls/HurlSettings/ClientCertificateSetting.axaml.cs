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
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class ClientCertificateSetting : BaseSettingControl<Collections.Settings.ClientCertificateSetting>
    {
        public ClientCertificateSetting(ILogger<ClientCertificateSetting> logger,
            INotificationService notificationService, MainWindow mainWindow) : base(mainWindow, logger,
            notificationService)
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Display file explorer to select a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonOpenCertificate_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                string? certificateFile = await this.DisplayOpenFilePickerSingle(
                    Localization.Localization.Setting_CaCertSetting_FilePicker_Title,
                    new[] { FilePickerFileTypes.All });
                if (!string.IsNullOrEmpty(certificateFile))
                {
                    _viewModel.CertificateFile = certificateFile;
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonOpenCertificate_Click));
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Display file explorer to select a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonOpenKey_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                string? keyFile = await this.DisplayOpenFilePickerSingle(
                    Localization.Localization.Setting_CaCertSetting_FilePicker_Title,
                    new[] { FilePickerFileTypes.All });
                if (!string.IsNullOrEmpty(keyFile))
                {
                    _viewModel.KeyFile = keyFile;
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonOpenKey_Click));
                _notificationService.Notify(ex);
            }
        }
    }
}