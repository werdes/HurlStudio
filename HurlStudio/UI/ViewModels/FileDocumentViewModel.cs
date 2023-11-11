using Dock.Model.Mvvm.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public class FileDocumentViewModel : Document
    {
        public FileDocumentViewModel()
        {
            this.CanFloat = false;
            this.CanPin = false;
        }
    }
}
