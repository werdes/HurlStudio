using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HurlStudio.Model.UiState
{
    public class UiState
    {
        private Dictionary<string, bool> _expandedCollectionExplorerComponents;
        private List<FileHistoryEntry> _fileHistoryEntries;
        private Dictionary<string, bool> _settingCollapsedStates;
        private Dictionary<string, bool> _settingSectionCollapsedStates;
        private List<string> _openedDocuments;
        private string? _activeDocument;
        private double _collectionExplorerProportion;
        private string? _activeEnvironmentFile;
        private Dictionary<string, bool> _settingEnabledStates;

        private bool _mainWindowIsMaximized;
        private Rectangle _mainWindowPosition;

        public UiState()
        {
            _expandedCollectionExplorerComponents = new Dictionary<string, bool>();
            _settingCollapsedStates = new Dictionary<string, bool>();
            _fileHistoryEntries = new List<FileHistoryEntry>();
            _settingSectionCollapsedStates = new Dictionary<string, bool>();
            _openedDocuments = new List<string>();
            _collectionExplorerProportion = .2D;
            _settingEnabledStates = new Dictionary<string, bool>();
        }

        [JsonPropertyName("expanded_collectionexplorer_components")]
        public Dictionary<string, bool> ExpandedCollectionExplorerComponents
        {
            get => _expandedCollectionExplorerComponents;
            set
            {
                _expandedCollectionExplorerComponents = value;
            }
        }

        [JsonPropertyName("mainwindow_position")]
        public Rectangle MainWindowPosition
        {
            get => _mainWindowPosition;
            set
            {
                _mainWindowPosition = value;
            }
        }

        [JsonPropertyName("mainwindow_is_maximized")]
        public bool MainWindowIsMaximized
        {
            get => _mainWindowIsMaximized;
            set
            {
                _mainWindowIsMaximized = value;
            }
        }

        [JsonPropertyName("history")]
        public List<FileHistoryEntry> FileHistoryEntries
        {
            get => _fileHistoryEntries;
            set
            {
                _fileHistoryEntries = value;
            }
        }

        [JsonPropertyName("setting_collapsed_states")]
        public Dictionary<string, bool> SettingCollapsedStates
        {
            get => _settingCollapsedStates;
            set
            {
                _settingCollapsedStates = value;
            }
        }

        [JsonPropertyName("setting_section_collapsed_states")]
        public Dictionary<string, bool> SettingSectionCollapsedStates
        {
            get => _settingSectionCollapsedStates;
            set
            {
                _settingSectionCollapsedStates = value;
            }
        }

        [JsonPropertyName("opened_documents")]
        public List<string> OpenedDocuments
        {
            get => _openedDocuments;
            set
            {
                _openedDocuments = value;
            }
        }

        [JsonPropertyName("active_document")]
        public string? ActiveDocument
        {
            get => _activeDocument;
            set
            {
                _activeDocument = value;
            }
        }

        [JsonPropertyName("collection_explorer_proportion")]
        public double CollectionExplorerProportion
        {
            get => _collectionExplorerProportion;
            set
            {
                _collectionExplorerProportion = value;
            }
        }

        [JsonPropertyName("active_environment")]
        public string? ActiveEnvironmentFile
        {
            get => _activeEnvironmentFile;
            set
            {
                _activeEnvironmentFile = value;
            }
        }

        [JsonPropertyName("setting_enabled_states")]
        public Dictionary<string, bool> SettingEnabledStates
        {
            get => _settingEnabledStates;
            set
            {
                _settingEnabledStates = value;
            }
        }
    }
}
