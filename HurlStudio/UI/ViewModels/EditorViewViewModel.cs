using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.UiState;
using HurlStudio.Services.UiState;
using HurlStudio.UI.Dock;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public class EditorViewViewModel : ViewModelBase
    {
        private ILogger _log;

        private ObservableCollection<CollectionContainer> _collections;
        private ObservableCollection<HurlEnvironment> _environments;
        private ObservableCollection<IDockable> _documents;
        private ObservableCollection<FileHistoryEntry> _fileHistoryEntries;
        private IDockable? _activeDocument;
        private IRootDock? _layout;
        private IUiStateService _uiStateService;

        public EditorViewViewModel(ILogger<EditorViewViewModel> logger) : base(typeof(EditorView))
        {
            _collections = new ObservableCollection<CollectionContainer>();
            _environments = new ObservableCollection<HurlEnvironment>();
            _fileHistoryEntries = new ObservableCollection<FileHistoryEntry>();
            _documents = new ObservableCollection<IDockable>();
            _activeDocument = null;
            _log = logger;
        }


        public ObservableCollection<CollectionContainer> Collections
        {
            get => _collections;
            set
            {
                _collections = value;
                Notify();
            }
        }

        public ObservableCollection<HurlEnvironment> Environments
        {
            get => _environments;
            set
            {
                _environments = value;
                Notify();
            }
        }

        public ObservableCollection<IDockable> Documents
        {
            get => _documents;
            set
            {
                _documents = value;
                Notify();
            }
        }

        public ObservableCollection<FileHistoryEntry> FileHistoryEntries
        {
            get => _fileHistoryEntries;
            set
            {
                _fileHistoryEntries = value;
                Notify();
            }
        }

        public IDockable? ActiveDocument
        {
            get => _activeDocument;
            set
            {
                _activeDocument = value;
                Notify();
            }
        }

        public IRootDock? Layout
        {
            get => _layout;
            set
            {
                _layout = value;
                Notify();
            }
        }
    }
}
