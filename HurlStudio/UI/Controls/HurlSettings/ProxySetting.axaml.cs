using Avalonia.Controls;
using HurlStudio.Collections.Settings;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class ProxySetting : ViewModelBasedControl<Collections.Settings.ProxySetting>
    {
        private Collections.Settings.ProxySetting? _proxySetting;

        public ProxySetting()
        {
            InitializeComponent();
        }

        protected override void SetViewModelInstance(Collections.Settings.ProxySetting viewModel)
        {
            _proxySetting = viewModel;
            this.DataContext = viewModel;
        }
    }
}
