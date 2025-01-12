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
    public class AddEnvironmentWindowViewModel : ViewModelBase
    {
        private AddEnvironmentViewViewModel? _addEnvironmentViewViewModel;
        private HurlEnvironment? _environment;

        public AddEnvironmentWindowViewModel() : base(typeof(AddEnvironmentWindow))
        {
        }

        public AddEnvironmentViewViewModel? AddEnvironmentViewViewModel
        {
            get => _addEnvironmentViewViewModel;
            set
            {
                _addEnvironmentViewViewModel = value;
                this.Notify();
            }
        }

        public HurlEnvironment? Environment
        {
            get => _environment;
            set
            {
                _environment = value;
                this.Notify();
            }
        }
    }
}
