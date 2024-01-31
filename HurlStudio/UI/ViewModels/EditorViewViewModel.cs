using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.UiState;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
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
        private DocumentDock? _documentDock;
        private Document? _activeDocument;
        private IRootDock? _layout;
        private bool _showEndOfLine;
        private bool _showWhitespace;
        private bool _wordWrap;
        private bool _canUndo;
        private bool _canRedo;
        private HurlEnvironment? _activeEnvironment;

        public EditorViewViewModel(ILogger<EditorViewViewModel> logger, IUserSettingsService userSettingsService) : base(typeof(EditorView))
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

        public DocumentDock? DocumentDock
        {
            get => _documentDock;
            set
            {
                _documentDock = value;
                Notify();
            }
        }

        public Document? ActiveDocument
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

        public bool ShowEndOfLine
        {
            get => _showEndOfLine;
            set
            {
                _showEndOfLine = value;
                Notify();
            }
        }

        public bool ShowWhitespace
        {
            get => _showWhitespace;
            set
            {
                _showWhitespace = value;
                Notify();
            }
        }

        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                Notify();
            }
        }

        public bool CanUndo
        {
            get => _canUndo;
            set
            {
                _canUndo = value;
                Notify();
            }
        }

        public bool CanRedo
        {
            get => _canRedo;
            set
            {
                _canRedo = value;
                Notify();
            }
        }

        public HurlEnvironment? ActiveEnvironment
        {
            get => _activeEnvironment;
            set
            {
                _activeEnvironment = value;
                Notify();
            }
        }
    }
}
