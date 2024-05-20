using CommunityToolkit.Mvvm.ComponentModel;
using HurlStudio.Model.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlContainers
{
    public abstract class HurlContainerBase : INotifyPropertyChanged
    {
        public event EventHandler<ControlSelectionChangedEventArgs>? ControlSelectionChanged;
        public event EventHandler<HurlContainerMovedEventArgs>? CollectionComponentMoved;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _selected;

        public HurlContainerBase()
        {
            _selected = false;
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                this.ControlSelectionChanged?.Invoke(this, new ControlSelectionChangedEventArgs(value));

                _selected = value;
                this.Notify();
            }
        }

        public virtual void Unselect()
        {
            _selected = false;
            this.Notify(nameof(this.Selected));
        }

        protected void On_CollectionComponentHierarchyBase_ControlUnselected(object? sender, ControlSelectionChangedEventArgs e)
            =>
                this.ControlSelectionChanged?.Invoke(sender, e);
        
        protected void On_CollectionComponentHierarchyBase_CollectionComponentMoved(object? sender, HurlContainerMovedEventArgs e)
            =>
                this.CollectionComponentMoved?.Invoke(sender, e);

        public void Move(HurlContainerBase target)
        {
            HurlContainerMovedEventArgs eventArgs = new HurlContainerMovedEventArgs(this, target);
            this.CollectionComponentMoved?.Invoke(this, eventArgs);
        }

        public abstract string GetId();
    }
}
