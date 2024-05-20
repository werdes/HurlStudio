using Avalonia;
using Avalonia.Controls;
using HurlStudio.Common.Utility;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Collection : CollectionExplorerControlBase<HurlCollectionContainer>
    {
        private HurlCollectionContainer CollectionContainer
        {
            get => (HurlCollectionContainer)this.GetValue(CollectionContainerProperty);
            set => this.SetValue(CollectionContainerProperty, value);
        }

        public static readonly StyledProperty<HurlCollectionContainer> CollectionContainerProperty =
            AvaloniaProperty.Register<Collection, HurlCollectionContainer>(nameof(CollectionContainer));

        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;

        public Collection(ILogger<Collection> logger, INotificationService notificationService, IEditorService editorService)
            : base(notificationService, logger)
        {
            _editorService = editorService;
            _log = logger;
            _notificationService = notificationService;

            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(HurlCollectionContainer viewModel)
        {
            this.CollectionContainer = viewModel;
        }

        /// <summary>
        /// Set DataContext to provided avalonia property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Collection_Initialized(object? sender, EventArgs e)
        {
            this.DataContext = this.CollectionContainer;
        }

        /// <summary>
        /// Toggle collapse state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (this.DataContext == null) return;
            this.CollectionContainer.Collapsed = !this.CollectionContainer.Collapsed;
        }

        /// <summary>
        /// Returns the collection container as bound element
        /// </summary>
        /// <returns></returns>
        protected override HurlContainerBase GetBoundCollectionComponent()
        {
            return this.CollectionContainer;
        }

        /// <summary>
        /// Opens a collection settings document
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            try
            {
                await _editorService.OpenCollectionSettings(this.CollectionContainer);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.OpenComponentDocument));
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Opens the folder containing the collection file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItem_RevealInExplorer_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (this.CollectionContainer == null) return;
            if (this.CollectionContainer.Collection == null) return;
            if (this.CollectionContainer.Collection.FileLocation == null) return;
            try
            {
                OSUtility.RevealFileInExplorer(this.CollectionContainer.Collection.FileLocation);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.OpenComponentDocument));
                _notificationService.Notify(ex);
            }
        }
    }
}
