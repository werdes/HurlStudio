using Avalonia;
using Avalonia.Platform;
using HurlStudio.UI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        private MainViewViewModel _mainViewViewModel;
        
        public MainViewViewModel MainViewViewModel
        {
            get => _mainViewViewModel;
            set
            {
                _mainViewViewModel = value;
                this.Notify();
            }
        }

        public MainWindowViewModel() : base(typeof(MainWindow))
        {
        }
    }
}
