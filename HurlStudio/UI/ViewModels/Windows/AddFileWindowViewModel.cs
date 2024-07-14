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
        private AddSettingViewViewModel? _addSettingViewViewModel;

        public AddFileWindowViewModel() : base(typeof(AddFileWindow))
        {
        }

        public AddSettingViewViewModel? AddSettingViewViewModel
        {
            get => _addSettingViewViewModel;
            set
            {
                _addSettingViewViewModel = value;
                this.Notify();
            }
        }
    }
}
