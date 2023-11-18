using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.ViewModels;

namespace HurlStudio.UI.Controls.Documents
{
    public partial class FileDocument : ViewModelBasedControl
    {
        public FileDocument() : base(typeof(FileDocumentViewModel))
        {
            InitializeComponent();
        }
    }
}
