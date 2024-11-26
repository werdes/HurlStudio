using HurlStudio.Collections.Model;
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
    public class AddFileWindowViewModel : ViewModelBase
    {
        private AddFileViewViewModel? _addFileViewViewModel;

        public AddFileWindowViewModel() : base(typeof(AddFileWindow))
        {
        }

        public AddFileViewViewModel? AddFileViewViewModel
        {
            get => _addFileViewViewModel;
            set
            {
                _addFileViewViewModel = value;
                this.Notify();
            }
        }
    }
}
