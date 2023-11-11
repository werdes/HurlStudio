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
        private MainWindowViewModel _mainWindowViewModel;
        private LoadingViewViewModel? _loadingViewViewModel;
        private EditorViewViewModel? _editorViewViewModel;

        private bool _initializationCompleted;


        public bool InitializationCompleted
        {
            get => _initializationCompleted;
            set
            {
                _initializationCompleted = value;
                Notify();
            }
        }


        public MainViewViewModel() : base(typeof(MainView))
        {
            _initializationCompleted = false;
        }

        public MainViewViewModel(MainWindowViewModel mainWindowViewModel) : this()
        {
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindowViewModel.MainViewViewModel = this;
            
            //_editorViewViewModel.RootViewModel = this;
            //_loadingViewViewModel.RootViewModel = this;
        }

        public MainWindowViewModel MainWindow
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
