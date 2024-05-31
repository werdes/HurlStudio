using HurlStudio.Collections.Model;
using HurlStudio.Common.Extensions;
using System;
using System.IO;

namespace HurlStudio.Model.HurlContainers
{
    public class HurlFolderContainer : HurlContainerHierarchyBase
    {
        private HurlCollectionContainer _collectionContainer;
        private HurlFolderContainer? _parentFolderContainer;
        private HurlFolder _folder;
        private string _absoluteLocation;
        private bool _found;

        public HurlFolderContainer(HurlCollectionContainer collectionContainer, HurlFolderContainer? parentFolder, HurlFolder folderSettings, string location) : base()
        {
            _collectionContainer = collectionContainer;
            _parentFolderContainer = parentFolder;
            _absoluteLocation = location;
            _found = true;
            _folder = folderSettings;
            _folder.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
        }

        public string AbsoluteLocation
        {
            get => _absoluteLocation;
            set
            {
                _absoluteLocation = value;
                this.Notify();
            }
        }

        public HurlFolder Folder
        {
            get => _folder;
            set
            {
                _folder = value;
                this.Notify();

                if (_folder != null)
                {
                    _folder.ComponentPropertyChanged -= this.On_HurlComponent_ComponentPropertyChanged;
                    _folder.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
                }
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

        public HurlFolderContainer? ParentFolderContainer
        {
            get => _parentFolderContainer;
            set
            {
                _parentFolderContainer = value;
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
            if (_collectionContainer.Collection.CollectionFileLocation == null) throw new ArgumentNullException(nameof(this.CollectionContainer.Collection.CollectionFileLocation));

            string id = _collectionContainer.GetId();
            string path = Path.GetRelativePath(Path.GetDirectoryName(_collectionContainer.Collection.CollectionFileLocation) ?? string.Empty, _absoluteLocation);
            return $"{id}#{path}".ToSha256Hash();
        }

        /// <summary>
        /// Returns the components' path
        /// </summary>
        /// <returns></returns>
        public override string GetPath()
        {
            return _absoluteLocation;
        }

        public override string ToString()
        {
            return $"{nameof(HurlFolderContainer)}: {this.GetId()} | {this.Folder}";
        }
    }
}
