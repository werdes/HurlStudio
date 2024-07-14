using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.Enums;
using HurlStudio.Model.EventArgs;
using HurlStudio.UI.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using System.IO;

namespace HurlStudio.Model.HurlSettings
{
    public class HurlSettingSection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<SettingSectionCollapsedChangedEventArgs>? SettingSectionCollapsedChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private OrderedObservableCollection<HurlSettingContainer> _settingContainers;
        private HurlContainerBase? _collectionComponent;
        private IEditorDocument? _document;
        private HurlSettingSectionType _sectionType;
        private string _sectionSubText;
        private bool _collapsed;

        public HurlSettingSection(IEditorDocument? document, HurlSettingSectionType sectionType, HurlContainerBase? collectionComponent)
        {
            _settingContainers = new OrderedObservableCollection<HurlSettingContainer>();

            _sectionType = sectionType;
            _collectionComponent = collectionComponent;
            _document = document;
            
            if(collectionComponent is HurlFolderContainer collectionFolder)
            {
                _sectionSubText = (collectionFolder.Folder?.FolderLocation ?? string.Empty).ConvertDirectorySeparator();
                collectionFolder.PropertyChanged += this.On_CollectionFolder_PropertyChanged;
            }
            else if(collectionComponent is HurlEnvironmentContainer environment)
            {
                _sectionSubText = environment.Environment.Name ?? string.Empty;
            }
            else if(collectionComponent is HurlCollectionContainer collection)
            {
                _sectionSubText = collection.Collection.Name ?? Path.GetFileName(collection.Collection.CollectionFileLocation);
            }
            else
            {
                _sectionSubText= string.Empty;
            }
        }

        /// <summary>
        /// Update subtext on property change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_CollectionFolder_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(sender != null && sender is HurlFolderContainer collectionFolder && e.PropertyName == nameof(collectionFolder.AbsoluteLocation))
            {
                this.SectionSubText = collectionFolder.Folder?.FolderLocation ?? string.Empty;
            }
        }

        public OrderedObservableCollection<HurlSettingContainer> SettingContainers
        {
            get => _settingContainers;
            set
            {
                _settingContainers = value;
                this.Notify();
            }
        }

        public HurlContainerBase? CollectionComponent
        {
            get => _collectionComponent;
            set
            {
                _collectionComponent = value;
                this.Notify();
            }
        }

        public HurlSettingSectionType SectionType
        {
            get => _sectionType;
            set
            {
                _sectionType = value;
                this.Notify();
            }
        }

        public string SectionSubText
        {
            get => _sectionSubText;
            set
            {
                _sectionSubText = value;
                this.Notify();
            }
        }

        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                _collapsed = value;
                this.Notify();
                this.SettingSectionCollapsedChanged?.Invoke(this, new SettingSectionCollapsedChangedEventArgs(_collapsed));
            }
        }
        
        public IEditorDocument? Document
        {
            get => _document;
        }

        public bool ShowContextMenu
        {
            get => this.ShowContextMenuPropertiesItem;
        }

        public bool ShowContextMenuPropertiesItem
        {
            get => _document is FileDocumentViewModel && _sectionType != HurlSettingSectionType.File;
        }

        /// <summary>
        /// Returns a unique ID for this section
        /// </summary>
        /// <returns></returns>
        public string GetId()
        {
            string id = _document?.GetId() + "#" + _collectionComponent?.GetId();
            return id.ToSha256Hash();
        }
    }
}
