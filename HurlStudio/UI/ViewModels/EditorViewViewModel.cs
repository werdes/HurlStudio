using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Common.UI;
using HurlStudio.Model.EventArgs;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.ViewModels.Documents;
using HurlStudio.UI.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private DocumentBase? _activeDocument;
        private DocumentBase? _previousDocument;
        private IRootDock? _layout;
        private bool _showEndOfLine;
        private bool _showWhitespace;
        private bool _wordWrap;
        private bool _canUndo;
        private bool _canRedo;
        private HurlEnvironmentContainer? _activeEnvironment;
        private ObservableStack<DocumentBase?> _documentHistory;
        private ObservableStack<DocumentBase?> _documentFuture;

        public EditorViewViewModel(ILogger<EditorViewViewModel> logger, IUserSettingsService userSettingsService) : base(typeof(EditorView))
        {
            _collections = new ObservableCollection<HurlCollectionContainer>();
            _environments = new ObservableCollection<HurlEnvironmentContainer>();
            _fileHistoryEntries = new ObservableCollection<FileHistoryEntry>();
            _documents = new ObservableCollection<IDockable>();
            _documentFuture = new ObservableStack<DocumentBase?>();
            _documentHistory = new ObservableStack<DocumentBase?>();
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

        public ObservableStack<DocumentBase?> DocumentHistory
        {
            get => _documentHistory;
            set
            {
                _documentHistory = value;
                this.Notify();
            }
        }

        public ObservableStack<DocumentBase?> DocumentFuture
        {
            get => _documentFuture;
            set
            {
                _documentFuture = value;
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

        public DocumentBase? ActiveDocument
        {
            get => _activeDocument;
            set
            {
                _previousDocument = _activeDocument;
                _activeDocument = value;
                this.Notify();
            }
        }

        public DocumentBase? PreviousDocument
        {
            get => _previousDocument;
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
