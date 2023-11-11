using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlUI.UI.ViewModels;

namespace HurlUI.UI.Controls.Tools
{
    public partial class CollectionExplorerTool : ControlBase
    {
        public CollectionExplorerTool() : base(typeof(CollectionExplorerToolViewModel))
        {
            InitializeComponent();
        }
    }
}
