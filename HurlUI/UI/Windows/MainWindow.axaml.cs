using Avalonia.Controls;
using HurlUI.UI.ViewModels;

namespace HurlUI.UI.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();
        }
    }
}
