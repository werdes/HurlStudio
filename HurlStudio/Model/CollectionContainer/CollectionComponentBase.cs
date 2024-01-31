using CommunityToolkit.Mvvm.ComponentModel;
using HurlStudio.Model.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.CollectionContainer
{
    public abstract class CollectionComponentBase : INotifyPropertyChanged
    {
        public event EventHandler<ControlSelectionChangedEventArgs>? ControlSelectionChanged;
        public event EventHandler<CollectionComponentMovedEventArgs>? CollectionComponentMoved;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _selected;

        public CollectionComponentBase()
        {
            _selected = false;
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                ControlSelectionChanged?.Invoke(this, new ControlSelectionChangedEventArgs(value));

                _selected = value;
                Notify();
            }
        }

        public virtual void Unselect()
        {
            _selected = false;
            Notify(nameof(Selected));
        }

        protected void On_CollectionComponentHierarchyBase_ControlUnselected(object? sender, ControlSelectionChangedEventArgs e)
            => ControlSelectionChanged?.Invoke(sender, e);
        
        protected void On_CollectionComponentHierarchyBase_CollectionComponentMoved(object? sender, CollectionComponentMovedEventArgs e)
            => CollectionComponentMoved?.Invoke(sender, e);

        public void Move(CollectionComponentBase target)
        {
            CollectionComponentMovedEventArgs eventArgs = new CollectionComponentMovedEventArgs(this, target);
            this.CollectionComponentMoved?.Invoke(this, eventArgs);
        }

        public abstract string GetId();
    }
}
