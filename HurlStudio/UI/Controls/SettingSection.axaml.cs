using Avalonia.Controls;
using HurlStudio.Model.HurlSettings;

namespace HurlStudio.UI.Controls
{
    public partial class SettingSection : ViewModelBasedControl<HurlSettingSection>
    {
        private HurlSettingSection? _settingSection;

        public SettingSection()
        {
            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(HurlSettingSection viewModel)
        {
            _settingSection = viewModel;
            this.DataContext = viewModel;
        }
    }
}
