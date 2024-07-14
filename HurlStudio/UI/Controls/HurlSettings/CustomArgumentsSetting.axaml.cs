using Avalonia.Controls;
using HurlStudio.Collections.Model.Containers;
using HurlStudio.Collections.Settings;
using HurlStudio.Common.Extensions;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class CustomArgumentsSetting : BaseSettingControl<Collections.Settings.CustomArgumentsSetting>
    {
        public CustomArgumentsSetting(MainWindow mainWindow, ILogger<NoProxySetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Add an empty row to the no-proxy list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonAddRow_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                if (_viewModel == null) return;
                if (_viewModel.Arguments == null) return;

                _viewModel.Arguments.Add(new CustomArgument(string.Empty));
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Remove a row from the no-proxy list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonRemoveRow_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                if (_viewModel == null) return;
                if (_viewModel.Arguments == null) return;
                if (sender is not Button source) return;
                if (source.DataContext is not CustomArgument argument) return;

                _viewModel.Arguments.Remove(argument);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}