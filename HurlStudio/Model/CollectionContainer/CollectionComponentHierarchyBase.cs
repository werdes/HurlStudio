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

namespace HurlStudio.Model.CollectionContainer
{
    public abstract class CollectionComponentHierarchyBase : CollectionComponentBase
    {
        private ObservableCollection<CollectionFile> _files;
        private ObservableCollection<CollectionFolder> _folders;
        private bool _collapsed;

        protected CollectionComponentHierarchyBase() : base()
        {
            _folders = new ObservableCollection<CollectionFolder>();
            _files = new ObservableCollection<CollectionFile>();
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
                    if(obj is CollectionComponentBase)
                    {
                        CollectionComponentBase component = (CollectionComponentBase)obj;
                        component.ControlSelectionChanged += this.On_CollectionComponentHierarchyBase_ControlUnselected;
                        component.CollectionComponentMoved += this.On_CollectionComponentHierarchyBase_CollectionComponentMoved;
                    }
                }
            }
        }


        public ObservableCollection<CollectionFolder> Folders
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

        public ObservableCollection<CollectionFile> Files
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

            foreach (CollectionFolder folder in _folders)
            {
                folder.Unselect();
            }

            foreach (CollectionFile file in _files)
            {
                file.Unselect();
            }

            base.Unselect();
        }
    }
}
