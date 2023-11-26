using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public abstract class CollectionExplorerControlBase : UserControl
    {
        public event EventHandler<CollectionComponentMovedEventArgs> CollectionComponentMoved;

        private volatile bool _pressed = false;
        private Point? _pressedPosition = null;

        public CollectionExplorerControlBase()
        {
            this.AddHandler(DragDrop.DropEvent, On_CollectionExplorerControlBase_Drop);
            this.AddHandler(DragDrop.DragEnterEvent, On_CollectionExplorerControlBase_DragEnter);
            this.AddHandler(DragDrop.DragLeaveEvent, On_CollectionExplorerControlBase_DragLeave);
            this.AddHandler(DragDrop.DragOverEvent, On_CollectionExplorerControlBase_DragOver);
        }

        protected abstract CollectionComponentBase GetBoundCollectionComponent();

        protected void On_CollectionExplorerControlBase_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            PointerPointProperties pointerProperties = e.GetCurrentPoint(this).Properties;
            _pressed = pointerProperties.IsLeftButtonPressed;
            if (_pressed)
            {
                _pressedPosition = e.GetCurrentPoint(this).Position;
            }

            CollectionComponentBase? component = this.GetBoundCollectionComponent();
            if (component != null)
            {
                component.Selected = true;
            }

            e.Handled = true;
        }

        protected void On_CollectionExplorerControlBase_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            _pressed = false;
            _pressedPosition = null;
        }

        /// <summary>
        /// Initiate DragDrop on pointer move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void On_CollectionExplorerControlBase_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (!_pressed || _pressedPosition == null) return;

            PointerPoint currentPoint = e.GetCurrentPoint(this);
            if (!currentPoint.Position.NearlyEquals(_pressedPosition.Value))
            {
                CollectionComponentBase? component = this.GetBoundCollectionComponent();
                if (component != null && component.GetType() != typeof(CollectionContainer))
                {
                    DataObject dataObject = new DataObject();
                    dataObject.Set(DataFormats.Files, component);
                    DragDropEffects dragResult = await DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Move);

                    _pressed = false;
                    _pressedPosition = null;
                }
            }
        }

        /// <summary>
        /// Select targeted element on drag enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_CollectionExplorerControlBase_DragEnter(object? sender, Avalonia.Input.DragEventArgs e)
        {
            bool allowDrop = DragDrop.GetAllowDrop(this);

            CollectionComponentBase? component = this.GetBoundCollectionComponent();
            if (component != null)
            {
                component.Selected = allowDrop;
            }

            e.Handled = true;
        }

        protected void On_CollectionExplorerControlBase_DragLeave(object? sender, Avalonia.Input.DragEventArgs e)
        {
            e.Handled = true;
            _pressed = false;
        }

        protected void On_CollectionExplorerControlBase_DragOver(object? sender, Avalonia.Input.DragEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// On drop of another collection component, fire the CollectionComponentMoved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_CollectionExplorerControlBase_Drop(object? sender, Avalonia.Input.DragEventArgs e)
        {
            CollectionComponentBase? source = (CollectionComponentBase?)(e.Data.Get(DataFormats.Files));

            if (source != null)
            {
                CollectionComponentMovedEventArgs collectionComponentMovedEventArgs = new CollectionComponentMovedEventArgs(source, this.GetBoundCollectionComponent());

                this.CollectionComponentMoved?.Invoke(this, collectionComponentMovedEventArgs);
            }
            e.Handled = true;
        }

        /// <summary>
        /// Relay the CollectionComponentMoved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_CollectionExplorerControlBase_CollectionComponentMoved(object sender, CollectionComponentMovedEventArgs e)
        {
            this.CollectionComponentMoved?.Invoke(sender, e);
        }
    }
}
