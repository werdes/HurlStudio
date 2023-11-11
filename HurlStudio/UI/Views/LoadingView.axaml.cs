using Avalonia.Controls;
using HurlStudio.UI.ViewModels;

namespace HurlStudio.UI.Views
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
