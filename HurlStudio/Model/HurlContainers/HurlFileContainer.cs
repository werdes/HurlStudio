using HurlStudio.Collections.Model;
using HurlStudio.Common.Extensions;
using HurlStudio.UI.Controls.CollectionExplorer;
using System;
using System.IO;

namespace HurlStudio.Model.HurlContainers
{
    public class HurlFileContainer : HurlContainerBase
    {
        private HurlFolderContainer _collectionFolder;
        private HurlCollectionContainer _collectionContainer;
        private HurlFile _file;
        private string _location;

        public HurlFileContainer(HurlFolderContainer collectionFolder, HurlFile fileSettings, string location)
        {
            _collectionFolder = collectionFolder;
            _collectionContainer = collectionFolder.CollectionContainer;
            _location = location;
            _file = fileSettings;
            _file.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
        }

        public HurlFolderContainer Folder
        {
            get => _collectionFolder;
            set
            {
                _collectionFolder = value;
                this.Notify();
            }
        }

        public HurlFile File
        {
            get => _file;
            set
            {
                _file = value;
                this.Notify();

                if (_file != null)
                {
                    _file.ComponentPropertyChanged -= this.On_HurlComponent_ComponentPropertyChanged;
                    _file.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
                }
            }
        }

        public HurlCollectionContainer Collection
        {
            get => _collectionContainer;
            set
            {
                _collectionContainer = value;
                this.Notify();
            }
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

        /// <summary>
        /// Returns a unique identifier for this file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if the containing folder is null</exception>
        public override string GetId()
        {
            if (_collectionFolder == null) throw new ArgumentNullException(nameof(this.Folder));

            string folderId = _collectionFolder.GetId();
            string fileName = Path.GetFileName(_location);
            return $"{folderId}#{fileName}".ToSha256Hash();
        }

        /// <summary>
        /// Returns the components' path
        /// </summary>
        /// <returns></returns>
        public override string GetPath()
        {
            return _location;
        }

        public override string ToString()
        {
            return $"{nameof(HurlFileContainer)}: {this.GetId()} | {this.File}";
        }
    }
}
