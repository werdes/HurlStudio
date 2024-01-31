using Avalonia;
using Avalonia.Controls;
using HurlStudio.Collections.Settings;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class SettingContainer : ViewModelBasedControl<Model.HurlSettings.HurlSettingContainer>
    {
        private Model.HurlSettings.HurlSettingContainer? _settingContainer;

        public SettingContainer()
        {
            InitializeComponent();
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
            if (_settingContainer != null)
            {
                _settingContainer.Collapsed = !_settingContainer.Collapsed;
            }
        }

        /// <summary>
        /// Enabled state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonEnable_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_settingContainer != null)
            {
                _settingContainer.Enabled = !_settingContainer.Enabled;
            }
        }
    }
}
