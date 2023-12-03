using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.ViewModels.Tools;

namespace HurlStudio.UI.Controls.Tools
{
    public partial class FileSettingsTool : ViewModelBasedControl<FileSettingsToolViewModel>
    {
        public FileSettingsTool()
        {
            InitializeComponent();
        }

        protected override void SetViewModelInstance(FileSettingsToolViewModel viewModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
