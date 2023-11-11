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
    public class ControlBase : UserControl
    {
        private Type? _attachedViewModelType = null;
        public Type? AttachedViewModelType { get => _attachedViewModelType; }

        public ControlBase(Type attachedViewModelType)
        {
            _attachedViewModelType = attachedViewModelType;
        }
    }
}
