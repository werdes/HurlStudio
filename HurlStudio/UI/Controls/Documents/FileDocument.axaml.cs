using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.ViewModels.Documents;

namespace HurlStudio.UI.Controls.Documents
{
    public partial class FileDocument : ViewModelBasedControl<FileDocumentViewModel>
    {
        public FileDocument()
        {
            InitializeComponent();
        }

        protected override void SetViewModelInstance(FileDocumentViewModel viewModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
