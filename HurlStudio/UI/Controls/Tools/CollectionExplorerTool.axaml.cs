using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.ViewModels;

namespace HurlStudio.UI.Controls.Tools
{
    public partial class CollectionExplorerTool : ControlBase
    {
        public CollectionExplorerTool() : base(typeof(CollectionExplorerToolViewModel))
        {
            InitializeComponent();
        }
    }
}
