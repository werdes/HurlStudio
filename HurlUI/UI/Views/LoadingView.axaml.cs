using Avalonia.Controls;
using HurlUI.UI.ViewModels;

namespace HurlUI.UI.Views
{
    public partial class LoadingView : ViewBase
    {
        public LoadingView() : base(typeof(LoadingViewViewModel))
        {
            InitializeComponent();
        }
    }
}
