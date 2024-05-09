using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.Model.CollectionContainer;
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

namespace HurlStudio.Model.HurlSettings
{
    public class HurlSettingSection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<SettingSectionCollapsedChangedEventArgs>? SettingSectionCollapsedChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private OrderedObservableCollection<HurlSettingContainer> _settingContainers;
        private CollectionComponentBase? _collectionComponent;
        private FileDocumentViewModel _document;
        private HurlSettingSectionType _sectionType;
        private string _sectionSubText;
        private bool _collapsed;

        public HurlSettingSection(FileDocumentViewModel document, HurlSettingSectionType sectionType, CollectionComponentBase? collectionComponent)
        {
            _settingContainers = new OrderedObservableCollection<HurlSettingContainer>();
            _sectionType = sectionType;
            _collectionComponent = collectionComponent;
            _document = document;
            
            if(collectionComponent is CollectionFolder collectionFolder)
            {
                _sectionSubText = collectionFolder.Folder?.Location ?? string.Empty;
                collectionFolder.PropertyChanged += this.On_CollectionFolder_PropertyChanged;
            }
            else if(collectionComponent is CollectionEnvironment environment)
            {
                _sectionSubText = environment.Environment.Name;
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
            if(sender != null && sender is CollectionFolder collectionFolder && e.PropertyName == nameof(collectionFolder.Location))
            {
                this.SectionSubText = collectionFolder.Folder?.Location ?? string.Empty;
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

        public CollectionComponentBase? CollectionComponent
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

        /// <summary>
        /// Returns a unique ID for this section
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string GetId()
        {
            //if (_collectionComponent == null) throw new ArgumentNullException(nameof(this.CollectionComponent));
            if (_document.File == null) throw new ArgumentNullException(nameof(_document.File));

            string id = _document.File.GetId() + "#" + _collectionComponent?.GetId();
            return id.ToSha256Hash();
        }
    }
}
