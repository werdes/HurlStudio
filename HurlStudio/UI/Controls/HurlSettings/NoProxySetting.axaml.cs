using Avalonia.Controls;
using HurlStudio.Collections.Model.Containers;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class NoProxySetting : BaseSettingControl<Collections.Settings.NoProxySetting>
    {
        public NoProxySetting(MainWindow mainWindow, ILogger<NoProxySetting> logger,
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
                if (_viewModel.NoProxyHosts == null) return;

                _viewModel.NoProxyHosts.Add(new NoProxyHost(string.Empty));
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_ButtonAddRow_Click));
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
                if (_viewModel.NoProxyHosts == null) return;
                if (sender is not Button source) return;
                if (source.DataContext is not NoProxyHost host) return;

                _viewModel.NoProxyHosts.Remove(host);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_ButtonRemoveRow_Click));
                _notificationService.Notify(ex);
            }
        }
    }
}