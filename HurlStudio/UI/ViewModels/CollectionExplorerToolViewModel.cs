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
