using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Tools
{
    public class FileSettingsToolViewModel : Tool
    {
        public FileSettingsToolViewModel()
        {
            CanClose = false;
            CanFloat = false;
            CanPin = false;
            Title = Localization.Localization.Dock_Tool_FileSettings_Title;
        }
    }
}
