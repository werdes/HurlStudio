using Avalonia.Controls;
using HurlStudio.UI.ViewModels;
using HurlStudio.Utility;

namespace HurlStudio.UI.Views
{
    public partial class LoadingView : ViewBase<LoadingViewViewModel>
    {
        public LoadingView(ControlLocator controlLocator) : base(null, controlLocator)
        {
            this.InitializeComponent();
        }
    }
}
