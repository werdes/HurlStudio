using Avalonia;
using Avalonia.Platform;
using HurlUI.UI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI.ViewModels
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
                Notify();
            }
        }

        public bool IsMac
        {
            get => _isMac;
            set
            {
                _isMac = value;
                Notify();
            }
        }

        public bool IsLinux
        {
            get => _isLinux;
            set
            {
                _isLinux = value;
                Notify();
            }
        }

        public MainViewViewModel MainViewViewModel
        {
            get => _mainViewViewModel;
            set
            {
                _mainViewViewModel = value;
                Notify();
            }
        }

        public MainWindowViewModel() : base(typeof(MainWindow))
        {
            this._isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            this._isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            this._isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
    }
}
