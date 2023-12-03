using Dock.Model.Mvvm.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Documents
{
    public class WelcomeDocumentViewModel : Document
    {
        private EditorViewViewModel _editorViewViewModel;

        public WelcomeDocumentViewModel(EditorViewViewModel editorViewViewModel)
        {
            CanFloat = false;
            CanPin = false;
            Title = Localization.Localization.Dock_Document_Welcome_Title;

            _editorViewViewModel = editorViewViewModel;
        }

        public EditorViewViewModel EditorViewViewModel
        {
            get => _editorViewViewModel;
            set => SetProperty(ref _editorViewViewModel, value);
        }
    }
}
