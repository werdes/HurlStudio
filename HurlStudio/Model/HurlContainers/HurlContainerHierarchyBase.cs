using HurlStudio.Model.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlContainers
{
    public abstract class HurlContainerHierarchyBase : HurlContainerBase
    {
        private ObservableCollection<HurlFileContainer> _files;
        private ObservableCollection<HurlFolderContainer> _folders;
        private bool _collapsed;

        protected HurlContainerHierarchyBase() : base()
        {
            _folders = new ObservableCollection<HurlFolderContainer>();
            _files = new ObservableCollection<HurlFileContainer>();
            _collapsed = false;

            _folders.CollectionChanged += this.On_CollectionComponent_CollectionChanged;
            _files.CollectionChanged += this.On_CollectionComponent_CollectionChanged;
        }

        private void On_CollectionComponent_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems != null && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(object obj in e.NewItems)
                {
                    if(obj is HurlContainerBase)
                    {
                        HurlContainerBase component = (HurlContainerBase)obj;
                        component.ControlSelectionChanged += this.On_CollectionComponentHierarchyBase_ControlUnselected;
                        component.CollectionComponentMoved += this.On_CollectionComponentHierarchyBase_CollectionComponentMoved;
                    }
                }
            }
        }


        public ObservableCollection<HurlFolderContainer> Folders
        {
            get => _folders;
            set
            {
                _folders = value;
                _folders.CollectionChanged -= this.On_CollectionComponent_CollectionChanged;
                _folders.CollectionChanged += this.On_CollectionComponent_CollectionChanged;
                this.Notify();
            }
        }

        public ObservableCollection<HurlFileContainer> Files
        {
            get => _files;
            set
            {
                _files = value;
                _files.CollectionChanged -= this.On_CollectionComponent_CollectionChanged;
                _files.CollectionChanged += this.On_CollectionComponent_CollectionChanged;
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
            }
        }
        
        /// <summary>
        /// Removes the selection on folders and their contents
        /// </summary>
        public override void Unselect()
        {

            foreach (HurlFolderContainer folder in _folders)
            {
                folder.Unselect();
            }

            foreach (HurlFileContainer file in _files)
            {
                file.Unselect();
            }

            base.Unselect();
        }
    }
}
