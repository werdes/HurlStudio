using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.EventArgs;
using HurlStudio.Services.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public abstract class CollectionExplorerControlBase<T> : ViewModelBasedControl<T>
    {
        private volatile bool _pressed = false;
        private Point? _pressedPosition = null;
        private INotificationService _notificationService;
        private ILogger _log;

        public CollectionExplorerControlBase(INotificationService notificationService, ILogger logger)
        {
            _notificationService = notificationService;
            _log = logger;

            this.AddHandler(DragDrop.DropEvent, this.On_CollectionExplorerControlBase_Drop);
            this.AddHandler(DragDrop.DragEnterEvent, this.On_CollectionExplorerControlBase_DragEnter);
            this.AddHandler(DragDrop.DragLeaveEvent, this.On_CollectionExplorerControlBase_DragLeave);
            this.AddHandler(DragDrop.DragOverEvent, this.On_CollectionExplorerControlBase_DragOver);
        }

        protected abstract CollectionComponentBase GetBoundCollectionComponent();
        protected abstract Task OpenComponentDocument();

        protected async void On_TitlePanel_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            PointerPointProperties pointerProperties = e.GetCurrentPoint(this).Properties;
            CollectionComponentBase? component = this.GetBoundCollectionComponent();

            _pressed = pointerProperties.IsLeftButtonPressed;
            if (_pressed)
            {
                _pressedPosition = e.GetCurrentPoint(this).Position;
            }

            if (component != null)
            {
                if (e.ClickCount == 2 && component.Selected)
                {
                    await this.OpenComponentDocument();
                }

                component.Selected = true;
            }

            e.Handled = true;
        }

        protected void On_CollectionExplorerControlBase_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _pressed = false;
            _pressedPosition = null;
        }

        /// <summary>
        /// Initiate DragDrop on pointer move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void On_CollectionExplorerControlBase_PointerMoved(object? sender, PointerEventArgs e)
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
        protected void On_CollectionExplorerControlBase_DragEnter(object? sender, DragEventArgs e)
        {
            bool allowDrop = DragDrop.GetAllowDrop(this);

            CollectionComponentBase? component = this.GetBoundCollectionComponent();
            if (component != null)
            {
                component.Selected = allowDrop;
            }

            e.Handled = true;
        }

        protected void On_CollectionExplorerControlBase_DragLeave(object? sender, DragEventArgs e)
        {
            e.Handled = true;
            _pressed = false;
        }

        protected void On_CollectionExplorerControlBase_DragOver(object? sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// On drop of another collection component, fire the CollectionComponentMoved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void On_CollectionExplorerControlBase_Drop(object? sender, DragEventArgs e)
        {
            try
            {
                CollectionComponentBase? source = (CollectionComponentBase?)(e.Data.Get(DataFormats.Files));

                if (source != null)
                {
                    source.Move(this.GetBoundCollectionComponent());
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_CollectionExplorerControlBase_Drop));
                _notificationService.Notify(ex);
            }
        }
    }
}
