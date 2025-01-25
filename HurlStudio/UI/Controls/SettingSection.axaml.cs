using Avalonia.Controls;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Services.Editor;
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
        private readonly IEditorService _editorService;
        private HurlSettingSection? _settingSection;

        public SettingSection(ILogger<SettingSection> logger, INotificationService notificationService, IEditorService editorService)
        {
            _log = logger;
            _notificationService = notificationService;
            _editorService = editorService;

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
                _log.LogException(ex);
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
                _log.LogException(ex);
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
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Open the corresponding properties document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemProperties_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingSection == null) return;
            if (_settingSection.CollectionComponent == null) return;

            try
            {
                _editorService.OpenPathDocument(_settingSection.CollectionComponent.GetPath());
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}
