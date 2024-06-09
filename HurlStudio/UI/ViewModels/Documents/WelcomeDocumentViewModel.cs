using Dock.Model.Mvvm.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Documents
{
    public class WelcomeDocumentViewModel : DocumentBase
    {
        private EditorViewViewModel _editorViewViewModel;

        public WelcomeDocumentViewModel(EditorViewViewModel editorViewViewModel)
        {
            this.CanFloat = false;
            this.CanPin = false;
            this.Title = Localization.Localization.Dock_Document_Welcome_Title;

            _editorViewViewModel = editorViewViewModel;
        }

        public EditorViewViewModel EditorViewViewModel
        {
            get => _editorViewViewModel;
            set => this.SetProperty(ref _editorViewViewModel, value);
        }
    }
}
