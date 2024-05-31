using Avalonia;
using Avalonia.Controls;
using HurlStudio.Common.Utility;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.EventArgs;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using HurlStudio.Services.UiState;
using HurlStudio.Common.Extensions;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Folder : CollectionExplorerControlBase<HurlFolderContainer>
    {
        private HurlFolderContainer? _collectionFolder;
        
        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private IUiStateService _uiStateService;

        public Folder(ILogger<Folder> logger, INotificationService notificationService, IEditorService editorService, IUiStateService uiStateService)
            : base(notificationService, logger)
        {
            _log = logger;
            _editorService = editorService;
            _notificationService = notificationService;
            _uiStateService = uiStateService;

            this.InitializeComponent();

        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(HurlFolderContainer viewModel)
        {
            _collectionFolder = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Folder_Initialized(object? sender, EventArgs e)
        {
        }

        /// <summary>
        /// Toggle the folder's collapse state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapse_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_collectionFolder == null) return;

            try
            {
                _collectionFolder.Collapsed = !_collectionFolder.Collapsed;
                _uiStateService.SetCollectionExplorerCollapseState(_collectionFolder.GetId(), _collectionFolder.Collapsed);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Returns the folder as bound element
        /// </summary>
        /// <returns></returns>
        protected override HurlContainerBase? GetBoundCollectionComponent()
        {
            return _collectionFolder;
        }

        /// <summary>
        /// Opens a folder settings page
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            if (_collectionFolder == null) return;

            try
            {
                await _editorService.OpenFolder(_collectionFolder.AbsoluteLocation);
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
            if (_collectionFolder == null) return;
            if (_collectionFolder.AbsoluteLocation == null) return;

            try
            {
                OSUtility.RevealFileInExplorer(_collectionFolder.AbsoluteLocation);
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
            if (_collectionFolder == null) return;

            try
            {
                await _editorService.OpenFolder(_collectionFolder.AbsoluteLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}
