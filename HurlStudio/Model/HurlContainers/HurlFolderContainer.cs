using HurlStudio.Collections.Model.Collection;
using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlContainers
{
    public class HurlFolderContainer : HurlContainerHierarchyBase
    {
        private HurlCollectionContainer _collectionContainer;
        private HurlFolderContainer? _parentFolder;
        private HurlFolder? _folder;
        private string _location;
        private bool _found;

        public HurlFolderContainer(HurlCollectionContainer collectionContainer, HurlFolderContainer? parentFolder, string location) : base()
        {
            _collectionContainer = collectionContainer;
            _parentFolder = parentFolder;
            _location = location;
            _found = true;
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                this.Notify();
            }
        }

        public HurlFolder? Folder
        {
            get => _folder;
            set
            {
                _folder = value;
                this.Notify();
            }
        }

        public HurlCollectionContainer CollectionContainer
        {
            get => _collectionContainer;
            set
            {
                _collectionContainer = value;
                this.Notify();
            }
        }

        public HurlFolderContainer? ParentFolder
        {
            get => _parentFolder;
            set
            {
                _parentFolder = value;
                this.Notify();
            }
        }

        public bool Found
        {
            get => _found;
            set
            {
                _found = value;
                this.Notify();
            }
        }

        /// <summary>
        /// Returns a unique identifier for this folder
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if the collection or its location is null</exception>
        public override string GetId()
        {
            if (_collectionContainer == null) throw new ArgumentNullException(nameof(this.CollectionContainer));
            if (_collectionContainer.Collection == null) throw new ArgumentNullException(nameof(this.CollectionContainer.Collection));
            if (_collectionContainer.Collection.FileLocation == null) throw new ArgumentNullException(nameof(this.CollectionContainer.Collection.FileLocation));

            string id = _collectionContainer.GetId();
            string path = Path.GetRelativePath(Path.GetDirectoryName(_collectionContainer.Collection.FileLocation) ?? string.Empty, _location);
            return $"{id}#{path}".ToSha256Hash();
        }

        public override string ToString()
        {
            return $"{nameof(HurlFolderContainer)}: {this.GetId()} | {this.Folder}";
        }
    }
}
