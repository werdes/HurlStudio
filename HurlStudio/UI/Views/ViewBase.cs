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

namespace HurlStudio.UI.Views
{
    public abstract class ViewBase<T> : ViewModelBasedControl<T>
    {
        private Windows.WindowBase? _window;

        public Windows.WindowBase? Window
        {
            get => _window;
            set
            {
                _window = value;
            }
        }
    }
}
