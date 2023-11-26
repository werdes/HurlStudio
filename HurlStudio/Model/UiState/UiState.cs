using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HurlStudio.Model.UiState
{
    public class UiState
    {
        private Dictionary<string, bool> _expandedCollectionExplorerComponents;

        public UiState()
        {
            _expandedCollectionExplorerComponents = new Dictionary<string, bool>();
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
    }
}
