using HurlStudio.Model.Enums;
using HurlStudio.Model.Notifications;
using HurlStudio.UI.ViewModels.Controls;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public class MainViewViewModel : ViewModelBase
    {
        private ViewFrameViewModel? _viewFrameViewModel;
        private MainWindowViewModel? _mainWindowViewModel;
        private LoadingViewViewModel? _loadingViewViewModel;
        private EditorViewViewModel? _editorViewViewModel;
        private bool _notificationsExpanded;
        private StatusBarStatus _statusBarStatus;
        private string _statusBarDetail;


        private ObservableCollection<Notification> _notifications;


        public MainViewViewModel() : base(typeof(MainView))
        {
            _notifications = new ObservableCollection<Notification>();

            _statusBarDetail = string.Empty;
            _statusBarStatus = StatusBarStatus.Idle;
        }

        public MainViewViewModel(MainWindowViewModel mainWindowViewModel) : this()
        {
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindowViewModel.MainViewViewModel = this;
        }

        public ViewFrameViewModel? ViewFrameViewModel
        {
            get => _viewFrameViewModel;
            set
            {
                _viewFrameViewModel = value;
                this.Notify();
            }
        }

        public MainWindowViewModel? MainWindow
        {
            get => _mainWindowViewModel;
            set
            {
                _mainWindowViewModel = value;
                this.Notify();
            }
        }

        public LoadingViewViewModel? LoadingView
        {
            get => _loadingViewViewModel;
            set
            {
                _loadingViewViewModel = value;
                this.Notify();
            }
        }

        public EditorViewViewModel? EditorView
        {
            get => _editorViewViewModel;
            set
            {
                _editorViewViewModel = value;
                this.Notify();
            }
        }


        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                this.Notify();
            }
        }

        public bool NotificationsExpanded
        {
            get => _notificationsExpanded;
            set
            {
                _notificationsExpanded = value;
                this.Notify();
            }
        }

        public string StatusBarDetail
        {
            get => _statusBarDetail;
            set
            {
                _statusBarDetail = value;
                this.Notify();
            }
        }

        public StatusBarStatus StatusBarStatus
        {
            get => _statusBarStatus;
            set
            {
                _statusBarStatus = value;
                this.Notify();
            }
        }
    }
}
