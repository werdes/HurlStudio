using Avalonia.Controls;
using HurlStudio.Collections.Settings;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class VariableSetting : ViewModelBasedControl<Collections.Settings.VariableSetting>
    {
        private Collections.Settings.VariableSetting? _variableSetting;

        public VariableSetting()
        {
            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(Collections.Settings.VariableSetting viewModel)
        {
            _variableSetting = viewModel;
            this.DataContext = viewModel;
        }
    }
}
