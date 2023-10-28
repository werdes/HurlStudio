using Avalonia.Controls;
using HurlUI.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI.Views
{
    public class ViewBase : UserControl
    {
        private Type _attachedViewModelType = null;
        public Type AttachedViewModelType { get => _attachedViewModelType; }

        public ViewBase(Type attachedViewModelType)
        {
            _attachedViewModelType = attachedViewModelType;
        }
    }
}
