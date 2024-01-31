using HurlStudio.Collections.Model.Collection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.CollectionContainer
{
    public class CollectionFile : CollectionComponentBase
    {
        private CollectionFolder _collectionFolder;
        private CollectionContainer _collectionContainer;
        private HurlFile? _file;
        private string _location;

        public CollectionFile(CollectionFolder collectionFolder, string location)
        {
            _collectionFolder = collectionFolder;
            _collectionContainer = collectionFolder.CollectionContainer;
            _location = location;
        }

        public CollectionFolder Folder
        {
            get => _collectionFolder;
            set
            {
                _collectionFolder = value;
                Notify();
            }
        }

        public HurlFile? File
        {
            get => _file;
            set
            {
                _file = value;
                Notify();
            }
        }

        public CollectionContainer Collection
        {
            get => _collectionContainer;
            set
            {
                _collectionContainer = value;
                Notify();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                Notify();
            }
        }

        /// <summary>
        /// Returns a unique identifier for this file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if the containing folder is null</exception>
        public override string GetId()
        {
            if (_collectionFolder == null) throw new ArgumentNullException(nameof(Folder));

            string id = _collectionFolder.GetId();
            string fileName = Path.GetFileName(_location);
            return $"{id}#{fileName}";
        }

        public override string ToString()
        {
            return $"{nameof(CollectionFile)}: {GetId()} | {File}";
        }
    }
}
