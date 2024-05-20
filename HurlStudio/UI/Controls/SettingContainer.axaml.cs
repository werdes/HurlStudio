using Avalonia;
using Avalonia.Controls;
using HurlStudio.Collections.Settings;

namespace HurlStudio.UI.Controls
{
    public partial class SettingContainer : ViewModelBasedControl<Model.HurlSettings.HurlSettingContainer>
    {
        private Model.HurlSettings.HurlSettingContainer? _settingContainer;

        public SettingContainer()
        {
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

            _settingContainer.Collapsed = !_settingContainer.Collapsed;
        }

        /// <summary>
        /// Enabled state (for file settings)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonEnable_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            _settingContainer.Setting.IsEnabled = !_settingContainer.Setting.IsEnabled;
        }

        /// <summary>
        /// Enabled state of the container (for folder, collection and environment settings)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonContainerEnable_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            _settingContainer.IsEnabled = !_settingContainer.IsEnabled;
        }

        /// <summary>
        /// Enabled state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonMoveUp_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            _settingContainer.MoveUp();
        }

        /// <summary>
        /// Enabled state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonMoveDown_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            _settingContainer.MoveDown();
        }

        /// <summary>
        /// Move the setting to the top of the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemMoveToTop_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            _settingContainer.MoveToTop();
        }

        /// <summary>
        /// Move the setting to the bottom of the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemMoveToBottom_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            _settingContainer.MoveToBottom();
        }

        /// <summary>
        /// Remove the setting from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItemDeleteSetting_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer == null) return;

            _settingContainer.Document.RemoveSetting(_settingContainer);
        }
    }
}
