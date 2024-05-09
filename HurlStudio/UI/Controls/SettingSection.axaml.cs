using Avalonia.Controls;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Tools;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Controls
{
    public partial class SettingSection : ViewModelBasedControl<HurlSettingSection>
    {
        private readonly ILogger _log;
        private readonly INotificationService _notificationService;
        private HurlSettingSection? _settingSection;

        public SettingSection(ILogger<SettingSection> logger, INotificationService notificationService)
        {
            _log = logger;
            _notificationService = notificationService;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(HurlSettingSection viewModel)
        {
            _settingSection = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Button click that expands all settings in this section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonExpandAll_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                this.SetCollapsedStateForSectionSettings(false);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonExpandAll_Click));
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Button click that collapses all settings in this section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapseAll_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                this.SetCollapsedStateForSectionSettings(true);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonCollapseAll_Click));
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Sets the collapsed state for all collections
        /// </summary>
        /// <param name="isCollapsed"></param>
        private void SetCollapsedStateForSectionSettings(bool isCollapsed)
        {
            if (_settingSection == null) return;
            if (_settingSection.SettingContainers == null) return;

            foreach (HurlSettingContainer settingContainer in _settingSection.SettingContainers)
            {
                settingContainer.Collapsed = isCollapsed;
            }
        }

        /// <summary>
        /// Toggles the collapsed state of the whole setting section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingSection == null) return;

            try
            {
                _settingSection.Collapsed = !_settingSection.Collapsed;
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonCollapseAll_Click));
                _notificationService.Notify(ex);
            }
        }
    }
}
