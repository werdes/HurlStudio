using Avalonia.Controls;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace HurlStudio.UI.Controls
{
    public abstract class ViewModelBasedControl<T> : ViewModelBasedControl
    {
        public Type? AttachedViewModelType => typeof(T);

        public ViewModelBasedControl()
        {
        }

        public override Type? GetAttachedViewModelType() => this.AttachedViewModelType;
        protected abstract void SetViewModelInstance(T viewModel);

        public override void SetViewModel(object viewModel)
        {
            this.SetViewModelInstance((T)viewModel);
        }
    }

    public abstract class ViewModelBasedControl : ControlBase
    {
        internal ViewModelBasedControl() { }
        public abstract Type? GetAttachedViewModelType();
        public abstract void SetViewModel(object viewModel);
    }
}
