using HurlStudio.Collections.Model;
using HurlStudio.Common.Extensions;
using HurlStudio.UI.Controls.CollectionExplorer;
using System;
using System.IO;

namespace HurlStudio.Model.HurlContainers
{
    public class HurlFileContainer : HurlContainerBase
    {
        private HurlFolderContainer _folderContainer;
        private HurlCollectionContainer _collectionContainer;
        private HurlFile _file;
        private string _absoluteLocation;

        public HurlFileContainer(HurlFolderContainer collectionFolder, HurlFile fileSettings, string location)
        {
            _folderContainer = collectionFolder;
            _collectionContainer = collectionFolder.CollectionContainer;
            _absoluteLocation = location;
            _file = fileSettings;
            _file.ComponentPropertyChanged += this.On_HurlComponent_ComponentPropertyChanged;
        }

        public HurlFolderContainer FolderContainer
        {
            get => _folderContainer;
            set
            {
                _folderContainer = value;
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

        public HurlCollectionContainer CollectionContainer
        {
            get => _collectionContainer;
            set
            {
                _collectionContainer = value;
                this.Notify();
            }
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

        /// <summary>
        /// Returns a unique identifier for this file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if the containing folder is null</exception>
        public override string GetId()
        {
            if (_folderContainer == null) throw new ArgumentNullException(nameof(this.FolderContainer));

            string folderId = _folderContainer.GetId();
            string fileName = Path.GetFileName(_absoluteLocation);
            return $"{folderId}#{fileName}".ToSha256Hash();
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
            return $"{nameof(HurlFileContainer)}: {this.GetId()} | {this.File}";
        }
    }
}
