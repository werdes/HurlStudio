using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlUI.UI.ViewModels;

namespace HurlUI.UI.Controls.Tools
{
    public partial class FileSettingsTool : ControlBase
    {
        public FileSettingsTool() : base(typeof(FileSettingsToolViewModel))
        {
            InitializeComponent();
        }
    }
}
