using Avalonia.Controls;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls
{
    public abstract class ViewModelBasedControl<T> : ViewModelBasedControl
    {
        public Type? AttachedViewModelType { get => typeof(T); }

        public ViewModelBasedControl()
        {
        }

        public override Type? GetAttachedViewModelType() => AttachedViewModelType;
        protected abstract void SetViewModelInstance(T viewModel);

        public override void SetViewModel(object viewModel)
        {
            SetViewModelInstance((T)viewModel);
        }
    }

    public abstract class ViewModelBasedControl : UserControl
    {
        internal ViewModelBasedControl() { }
        public abstract Type? GetAttachedViewModelType();
        public abstract void SetViewModel(object viewModel);
    }
}
