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

namespace HurlStudio.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isWindows = false;
        private bool _isMac = false;
        private bool _isLinux = false;
        private MainViewViewModel _mainViewViewModel;

        public bool IsWindows
        {
            get => _isWindows;
            set
            {
                _isWindows = value;
                this.Notify();
            }
        }

        public bool IsMac
        {
            get => _isMac;
            set
            {
                _isMac = value;
                this.Notify();
            }
        }

        public bool IsLinux
        {
            get => _isLinux;
            set
            {
                _isLinux = value;
                this.Notify();
            }
        }

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
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            _isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            _isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
    }
}
