using Avalonia.Controls;
using HurlUI.UI.ViewModels;

namespace HurlUI.UI.Views
{
    public partial class LoadingView : ViewBase
    {
        public LoadingView(LoadingViewViewModel viewModel) : base(typeof(LoadingViewViewModel))
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }
    }
}
