using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.ViewModels.Documents;

namespace HurlStudio.UI.Controls.Documents
{
    public partial class WelcomeDocument : ViewModelBasedControl<WelcomeDocumentViewModel>
    {
        private WelcomeDocumentViewModel? _viewModel;

        public WelcomeDocument()
        {
            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(WelcomeDocumentViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }
    }
}
