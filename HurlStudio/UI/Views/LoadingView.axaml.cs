using Avalonia.Controls;
using HurlStudio.UI.ViewModels;

namespace HurlStudio.UI.Views
{
    public partial class LoadingView : ViewBase<LoadingViewViewModel>
    {
        private LoadingViewViewModel? _viewModel;

        public LoadingView() 
        {
            InitializeComponent();
        }

        protected override void SetViewModelInstance(LoadingViewViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }
    }
}
