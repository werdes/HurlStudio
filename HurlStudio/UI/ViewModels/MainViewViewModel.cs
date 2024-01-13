using HurlStudio.Model.Notifications;
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
    public class MainViewViewModel : ViewModelBase, INavigatableViewModel
    {
        private MainWindowViewModel? _mainWindowViewModel;
        private LoadingViewViewModel? _loadingViewViewModel;
        private EditorViewViewModel? _editorViewViewModel;
        private bool _notificationsExpanded;


        private bool _initializationCompleted;
        private ObservableCollection<Notification> _notifications;


        public MainViewViewModel() : base(typeof(MainView))
        {
            _initializationCompleted = false;
            _notifications = new ObservableCollection<Notification>();
        }

        public MainViewViewModel(MainWindowViewModel mainWindowViewModel) : this()
        {
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindowViewModel.MainViewViewModel = this;
        }

        public MainWindowViewModel? MainWindow
        {
            get => _mainWindowViewModel;
            set
            {
                _mainWindowViewModel = value;
                Notify();
            }
        }

        public LoadingViewViewModel? LoadingView
        {
            get => _loadingViewViewModel;
            set
            {
                _loadingViewViewModel = value;
                Notify();
            }
        }

        public EditorViewViewModel? EditorView
        {
            get => _editorViewViewModel;
            set
            {
                _editorViewViewModel = value;
                Notify();
            }
        }

        public bool InitializationCompleted
        {
            get => _initializationCompleted;
            set
            {
                _initializationCompleted = value;
                Notify();
            }
        }

        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                Notify();
            }
        }

        public bool NotificationsExpanded
        {
            get => _notificationsExpanded;
            set
            {
                _notificationsExpanded = value;
                Notify();
            }
        }

        /// <summary>
        /// Returns a list of available view models for navigation within a view frame
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ViewModelBase> GetNavigationTargets()
        {
            return new List<ViewModelBase>()
            {
                LoadingView,
                EditorView
            };
        }
    }
}
