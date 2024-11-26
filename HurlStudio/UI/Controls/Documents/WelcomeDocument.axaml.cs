using Avalonia.Controls;
using Avalonia.Interactivity;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.MessageBox;
using HurlStudio.UI.ViewModels.Documents;

namespace HurlStudio.UI.Controls.Documents
{
    public partial class WelcomeDocument : ViewModelBasedControl<WelcomeDocumentViewModel>
    {
        private WelcomeDocumentViewModel? _viewModel;

        public WelcomeDocument()
        {
            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(WelcomeDocumentViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        private async void On_ButtonNewCollection_Click(object? sender, RoutedEventArgs e)
        {
            if(_window == null) return;
            //
            // MessageBoxResult result1 = await MessageBox2.ShowInfoDialog(_window, "Hello", "Hello2");
            // string? result2 = await MessageBox2.AskInputDialog(_window, "Hello", "Hello2", "abc", Model.Enums.Icon.Rename);
        }
    }
}
