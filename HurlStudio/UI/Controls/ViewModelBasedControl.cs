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
    public class ViewModelBasedControl : UserControl
    {
        private Type? _attachedViewModelType = null;
        public Type? AttachedViewModelType { get => _attachedViewModelType; }

        public ViewModelBasedControl(Type attachedViewModelType)
        {
            _attachedViewModelType = attachedViewModelType;
        }
    }
}
