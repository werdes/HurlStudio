using Avalonia;
using Avalonia.Controls;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.Utility;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UiState;
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
        private HurlCollectionContainer? _collectionContainer;

        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private IUiStateService _uiStateService;

        public Collection(ILogger<Collection> logger, INotificationService notificationService, IEditorService editorService, IUiStateService uiStateService)
            : base(notificationService, logger)
        {
            _editorService = editorService;
            _uiStateService = uiStateService;
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
            _collectionContainer = viewModel;
            this.DataContext = _collectionContainer;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Collection_Initialized(object? sender, EventArgs e)
        {

        }

        /// <summary>
        /// Toggle collapse state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_collectionContainer == null) return;

            try
            {
                _collectionContainer.Collapsed = !_collectionContainer.Collapsed;
                _uiStateService.SetCollectionExplorerCollapseState(_collectionContainer.GetId(), _collectionContainer.Collapsed);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Returns the collection container as bound element
        /// </summary>
        /// <returns></returns>
        protected override HurlContainerBase? GetBoundCollectionComponent()
        {
            return _collectionContainer;
        }

        /// <summary>
        /// Opens a collection settings document
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            if (_collectionContainer == null) return;
            try
            {
                await _editorService.OpenCollection(_collectionContainer.Collection.CollectionFileLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
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
            if (_collectionContainer == null) return;
            if (_collectionContainer.Collection == null) return;
            if (_collectionContainer.Collection.CollectionFileLocation == null) return;

            try
            {
                OSUtility.RevealFileInExplorer(_collectionContainer.Collection.CollectionFileLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Opens the collection document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Properties_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_collectionContainer == null) return;

            try
            {
                await _editorService.OpenCollection(_collectionContainer.Collection.CollectionFileLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}
