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
        private bool _mainWindowIsMaximized;
        private Rectangle _mainWindowPosition;

        public UiState()
        {
            _expandedCollectionExplorerComponents = new Dictionary<string, bool>();
            _fileHistoryEntries = new List<FileHistoryEntry>();
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
    }
}
