using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public class FileSettingsToolViewModel : Tool
    {
        public FileSettingsToolViewModel()
        {
            this.CanClose = false;
            this.CanFloat = false;
            this.CanPin = false;
            this.Title = Localization.Localization.Dock_Tool_FileSettings_Title;
        }
    }
}
