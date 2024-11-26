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
    public partial class VariablesFileSetting : BaseSettingControl<Collections.Settings.VariablesFileSetting>
    {
        public VariablesFileSetting(ILogger<VariablesFileSetting> logger, INotificationService notificationService,
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
                ReadOnlyCollection<FilePickerFileType> fileTypeFilters = new ReadOnlyCollection<FilePickerFileType>(
                    new List<FilePickerFileType>()
                    {
                        FilePickerFileTypes.All
                    });
                string? file = await this.DisplayOpenFilePickerSingle(
                    Localization.Localization.Setting_VariablesFileSetting_FilePicker_Title,
                    fileTypeFilters.ToArray());

                if (!string.IsNullOrEmpty(file))
                {
                    _viewModel.File = file;
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