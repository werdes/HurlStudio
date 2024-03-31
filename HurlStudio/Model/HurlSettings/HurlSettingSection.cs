using HurlStudio.Common.UI;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlSettings
{
    public class HurlSettingSection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private OrderedObservableCollection<HurlSettingContainer> _settingContainers;
        private CollectionComponentBase? _collectionComponent;
        private HurlSettingSectionType _sectionType;
        private string _sectionSubText;

        public HurlSettingSection(HurlSettingSectionType sectionType, CollectionComponentBase? collectionComponent)
        {
            _settingContainers = new OrderedObservableCollection<HurlSettingContainer>();
            _sectionType = sectionType;
            _collectionComponent = collectionComponent;
            
            if(collectionComponent is CollectionFolder collectionFolder)
            {
                _sectionSubText = collectionFolder.Location;
                collectionFolder.PropertyChanged += On_CollectionFolder_PropertyChanged;
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
                this.SectionSubText = collectionFolder.Location;
            }
        }

        public OrderedObservableCollection<HurlSettingContainer> SettingContainers
        {
            get => _settingContainers;
            set
            {
                _settingContainers = value;
                Notify();
            }
        }

        public CollectionComponentBase? CollectionComponent
        {
            get => _collectionComponent;
            set
            {
                _collectionComponent = value;
                Notify();
            }
        }

        public HurlSettingSectionType SectionType
        {
            get => _sectionType;
            set
            {
                _sectionType = value;
                Notify();
            }
        }

        public string SectionSubText
        {
            get => _sectionSubText;
            set
            {
                _sectionSubText = value;
                Notify();
            }
        }
    }
}
