using Dock.Model.Mvvm.Controls;
using HurlUI.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI.ViewModels
{
    public class FileSettingsToolViewModel : Tool
    {
        public FileSettingsToolViewModel()
        {
            this.CanClose = false;
            this.CanFloat = false;
            this.CanPin = false;
            this.Title = Localization.Localization.View_Editor_Dock_FileSettings_Title;
        }
    }
}
