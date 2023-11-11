using Dock.Model.Mvvm.Controls;
using HurlUI.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI.ViewModels
{
    public class CollectionExplorerToolViewModel : Tool
    {
        private EditorViewViewModel _viewModel;

        public CollectionExplorerToolViewModel(EditorViewViewModel editorViewViewModel)
        {
            this.CanClose = false;
            this.CanFloat = false;
            this.CanPin = false;

            this._viewModel = editorViewViewModel;
        }
    }
}
