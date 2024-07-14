using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Windows
{
    public class EditTemplateWindowViewModel : ViewModelBase
    {
        private EditTemplateViewViewModel? _editTemplateViewViewModel;
        private HurlFileTemplateContainer? _templateContainer;

        public EditTemplateWindowViewModel() : base(typeof(EditTemplateWindow))
        {
        }

        public EditTemplateViewViewModel? EditTemplateViewViewModel
        {
            get => _editTemplateViewViewModel;
            set
            {
                _editTemplateViewViewModel = value;
                this.Notify();
            }
        }

        public HurlFileTemplateContainer? TemplateContainer
        {
            get => _templateContainer;
            set
            {
                _templateContainer = value;
                this.Notify();
            }
        }
    }
}
