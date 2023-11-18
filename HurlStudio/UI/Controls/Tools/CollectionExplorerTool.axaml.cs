using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels;

namespace HurlStudio.UI.Controls.Tools
{
    public partial class CollectionExplorerTool : ViewModelBasedControl
    {
        public CollectionExplorerTool() : base(typeof(CollectionExplorerToolViewModel))
        {
            InitializeComponent();
        }
    }
}
