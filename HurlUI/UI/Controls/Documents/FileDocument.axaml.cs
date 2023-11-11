using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlUI.UI.ViewModels;

namespace HurlUI.UI.Controls.Documents
{
    public partial class FileDocument : ControlBase
    {
        public FileDocument() : base(typeof(FileDocumentViewModel))
        {
            InitializeComponent();
        }
    }
}
