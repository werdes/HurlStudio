using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.ViewModels.Documents;

namespace HurlStudio.UI.Controls.Documents
{
    public partial class WelcomeDocument : ViewModelBasedControl<WelcomeDocumentViewModel>
    {
        public WelcomeDocument()
        {
            InitializeComponent();
        }

        protected override void SetViewModelInstance(WelcomeDocumentViewModel viewModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
