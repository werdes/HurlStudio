using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.UiState
{
    public class FileHistoryEntry
    {
        private string _location;
        private DateTime _lastOpened;

        public FileHistoryEntry(string location, DateTime lastOpened)
        {
            _location = location;
            _lastOpened = lastOpened;
        }

        public string Location
        {
            get => _location;
            set => _location = value;
        }

        public DateTime LastOpened
        {
            get => _lastOpened;
            set => _lastOpened = value;
        }
    }
}
