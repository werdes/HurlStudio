using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public class CollectionExplorerToolViewModel : Tool
    {
        private EditorViewViewModel _editorViewModel;
        private bool _isEnabled = false;

        public CollectionExplorerToolViewModel(EditorViewViewModel editorViewViewModel)
        {
            this.CanClose = false;
            this.CanFloat = false;
            this.CanPin = false;

            _editorViewModel = editorViewViewModel;
        }

        public EditorViewViewModel EditorViewModel => _editorViewModel;
        
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

    }
}
