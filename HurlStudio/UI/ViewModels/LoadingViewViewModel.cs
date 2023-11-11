using HurlStudio.Model.Enums;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public class LoadingViewViewModel : ViewModelBase
    {
        private LoadingViewStep? _currentActivity;

        public LoadingViewStep? CurrentActivity
        {
            get => _currentActivity; 
            set
            {
                _currentActivity = value;
                Notify();
            }
        }

        public LoadingViewViewModel() : base(typeof(LoadingView))
        {
        }
    }
}
