using Avalonia.Controls;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HurlStudio.UI.ViewModels;

namespace HurlStudio.UI.Windows
{
    public class WindowBase<T> : WindowBase where T : ViewModelBase
    {
        protected T? _viewModel;

        public T? ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                this.DataContext = _viewModel;
            }
        }

        public override T? GetViewModel()
        {
            return _viewModel;
        }
    }

    public abstract class WindowBase : Window
    {
        public abstract ViewModelBase? GetViewModel();
    }
}
