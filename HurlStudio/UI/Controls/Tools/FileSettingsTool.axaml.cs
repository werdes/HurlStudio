using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.ViewModels.Tools;

namespace HurlStudio.UI.Controls.Tools
{
    public partial class FileSettingsTool : ViewModelBasedControl<FileSettingsToolViewModel>
    {
        private FileSettingsToolViewModel? _viewModel; 

        public FileSettingsTool()
        {
            InitializeComponent();
            _viewModel = null;
        }

        protected override void SetViewModelInstance(FileSettingsToolViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }
    }
}
