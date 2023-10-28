using Avalonia.Controls;
using HurlUI.UI.ViewModels;

namespace HurlUI.UI.Views
{
    public partial class EditorView : ViewBase
    {
        public EditorView() : base(typeof(EditorViewViewModel))
        {
            InitializeComponent();
        }
    }
}
