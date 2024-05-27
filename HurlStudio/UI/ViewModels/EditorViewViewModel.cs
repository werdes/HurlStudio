using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.EventArgs;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;

namespace HurlStudio.UI.ViewModels
{
    public class EditorViewViewModel : ViewModelBase
    {
        public event EventHandler<ActiveEnvironmentChangedEventArgs>? ActiveEnvironmentChanged;

        private ILogger _log;

        private ObservableCollection<HurlCollectionContainer> _collections;
        private ObservableCollection<HurlEnvironmentContainer> _environments;
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
        private HurlEnvironmentContainer? _activeEnvironment;

        public EditorViewViewModel(ILogger<EditorViewViewModel> logger, IUserSettingsService userSettingsService) : base(typeof(EditorView))
        {
            _collections = new ObservableCollection<HurlCollectionContainer>();
            _environments = new ObservableCollection<HurlEnvironmentContainer>();
            _fileHistoryEntries = new ObservableCollection<FileHistoryEntry>();
            _documents = new ObservableCollection<IDockable>();
            _activeDocument = null;
            _log = logger;

        }


        public ObservableCollection<HurlCollectionContainer> Collections
        {
            get => _collections;
            set
            {
                _collections = value;
                this.Notify();
            }
        }

        public ObservableCollection<HurlEnvironmentContainer> Environments
        {
            get => _environments;
            set
            {
                _environments = value;
                this.Notify();
            }
        }

        public ObservableCollection<IDockable> Documents
        {
            get => _documents;
            set
            {
                _documents = value;
                this.Notify();
            }
        }

        public ObservableCollection<FileHistoryEntry> FileHistoryEntries
        {
            get => _fileHistoryEntries;
            set
            {
                _fileHistoryEntries = value;
                this.Notify();
            }
        }

        public DocumentDock? DocumentDock
        {
            get => _documentDock;
            set
            {
                _documentDock = value;
                this.Notify();
            }
        }

        public Document? ActiveDocument
        {
            get => _activeDocument;
            set
            {
                _activeDocument = value;
                this.Notify();
            }
        }

        public IRootDock? Layout
        {
            get => _layout;
            set
            {
                _layout = value;
                this.Notify();
            }
        }

        public bool ShowEndOfLine
        {
            get => _showEndOfLine;
            set
            {
                _showEndOfLine = value;
                this.Notify();
            }
        }

        public bool ShowWhitespace
        {
            get => _showWhitespace;
            set
            {
                _showWhitespace = value;
                this.Notify();
            }
        }

        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                this.Notify();
            }
        }

        public bool CanUndo
        {
            get => _canUndo;
            set
            {
                _canUndo = value;
                this.Notify();
            }
        }

        public bool CanRedo
        {
            get => _canRedo;
            set
            {
                _canRedo = value;
                this.Notify();
            }
        }

        public HurlEnvironmentContainer? ActiveEnvironment
        {
            get => _activeEnvironment;
            set
            {
                _activeEnvironment = value;
                this.Notify();
                this.ActiveEnvironmentChanged?.Invoke(this, new ActiveEnvironmentChangedEventArgs(value));
            }
        }
    }
}
