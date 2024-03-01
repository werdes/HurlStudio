using Avalonia.Controls;
using HurlStudio.Collections.Settings;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class AwsSigV4Setting : ViewModelBasedControl<Collections.Settings.AwsSigV4Setting>
    {
        private Collections.Settings.AwsSigV4Setting? _awsSigV4Setting;

        public AwsSigV4Setting()
        {
            InitializeComponent();
        }

        protected override void SetViewModelInstance(Collections.Settings.AwsSigV4Setting viewModel)
        {
            _awsSigV4Setting = viewModel;
            this.DataContext = viewModel;
        }
    }
}
