using Avalonia.Controls;
using HurlStudio.Model.HurlSettings;

namespace HurlStudio.UI.Controls
{
    public partial class SettingTypeContainer : ViewModelBasedControl<HurlSettingTypeContainer>
    {
        private HurlSettingTypeContainer? _viewModel;

        public SettingTypeContainer()
        {
            InitializeComponent();
        }

        protected override void SetViewModelInstance(HurlSettingTypeContainer viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Select the container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_SettingTypeContainer_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_viewModel == null) return;

            _viewModel.Select();
        }
    }
}
