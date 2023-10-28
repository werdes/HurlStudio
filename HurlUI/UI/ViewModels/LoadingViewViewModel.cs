using HurlUI.UI.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI.ViewModels
{
    public class LoadingViewViewModel : ViewModelBase
    {
        private string? _dummyText;

        public string? DummyText
        {
            get => _dummyText; 
            set
            {
                _dummyText = value;
                Notify();
            }
        }

        public LoadingViewViewModel(ILogger<LoadingViewViewModel> logger) : base(typeof(LoadingView))
        {
            logger.LogDebug("LoadingViewViewModel init");
        }
    }
}
