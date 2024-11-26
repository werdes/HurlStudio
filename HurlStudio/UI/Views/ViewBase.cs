using Avalonia.Controls;
using HurlStudio.UI.ViewModels;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HurlStudio.UI.Controls;
using HurlStudio.Utility;

namespace HurlStudio.UI.Views
{
    public abstract class ViewBase<T> : ViewModelBasedControl<T>, IView where T: ViewModelBase
    {
        protected T? _viewModel;
        protected ControlLocator? _controlLocator;

        protected ViewBase(T? viewModel, ControlLocator? controlLocator)
        {
            _viewModel = viewModel; 
            _controlLocator = controlLocator;

            if (_viewModel != null)
            {
                this.DataContext = viewModel;
                _viewModel.View = this;
            }

            if (_controlLocator != null)
            {
                this.DataTemplates.Add(_controlLocator);
            }
        }

        protected override void SetViewModelInstance(T viewModel)
        {
            _viewModel = viewModel;
            _viewModel.View = this;
            this.DataContext = _viewModel;
        }
        
        public override void SetWindow(Windows.WindowBase window)
        {
            base.SetWindow(window);

            if (_controlLocator != null)
            {
                _controlLocator.Window = window;
            }
        }
    }
}
