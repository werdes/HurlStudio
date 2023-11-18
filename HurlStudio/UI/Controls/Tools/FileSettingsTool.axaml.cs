using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.ViewModels;

namespace HurlStudio.UI.Controls.Tools
{
    public partial class FileSettingsTool : ViewModelBasedControl
    {
        public FileSettingsTool() : base(typeof(FileSettingsToolViewModel))
        {
            InitializeComponent();
        }
    }
}
