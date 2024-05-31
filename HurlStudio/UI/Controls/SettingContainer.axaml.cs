using Avalonia;
using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Services.Notifications;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace HurlStudio.UI.Controls
{
    public partial class SettingContainer : ViewModelBasedControl<Model.HurlSettings.HurlSettingContainer>
    {
        private Model.HurlSettings.HurlSettingContainer? _settingContainer;
        private ILogger _log;
        private INotificationService _notificationService;

        public SettingContainer(ILogger<SettingContainer> logger, INotificationService notificationService)
        {
            _log = logger;
            _notificationService = notificationService;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(Model.HurlSettings.HurlSettingContainer viewModel)
        {
            _settingContainer = viewModel;
            this.DataContext = _settingContainer;
        }

        /// <summary>
        /// Collapse state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                _settingContainer.Collapsed = !_settingContainer.Collapsed;
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Enabled state (for file settings)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonEnable_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                _settingContainer.Setting.IsEnabled = !_settingContainer.Setting.IsEnabled;
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Enabled state of the container (for folder, collection and environment settings)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonContainerEnable_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                _settingContainer.IsEnabled = !_settingContainer.IsEnabled;
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Enabled state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonMoveUp_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                _settingContainer.MoveUp();
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Enabled state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonMoveDown_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                _settingContainer.MoveDown();
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Move the setting to the top of the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemMoveToTop_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                _settingContainer.MoveToTop();
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Move the setting to the bottom of the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemMoveToBottom_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                _settingContainer.MoveToBottom();
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Remove the setting from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemDeleteSetting_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                _settingContainer.Document.RemoveSetting(_settingContainer);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Duplicate a file setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemDuplicate_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                int currIdx = _settingContainer.SettingSection.SettingContainers.IndexOf(_settingContainer);

                BaseSetting duplicate = (BaseSetting)_settingContainer.Setting.Duplicate();

                _settingContainer.Document.AddSetting(new HurlSettingContainer(_settingContainer.Document, _settingContainer.SettingSection, duplicate, false, true, EnableType.Setting), currIdx + 1);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }


        /// <summary>
        /// Copy a setting from an inherited section to file settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemRedefineInFileSettings_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            try
            {
                BaseSetting duplicate = (BaseSetting)_settingContainer.Setting.Duplicate();
                HurlSettingSection? settingSection = _settingContainer.Document.SettingSections.FirstOrDefault(x => x.SectionType == Model.Enums.HurlSettingSectionType.File);

                if (settingSection == null) return;

                _settingContainer.Document.AddSetting(new HurlSettingContainer(_settingContainer.Document, settingSection, duplicate, false, true, EnableType.Setting));
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

    }
}
