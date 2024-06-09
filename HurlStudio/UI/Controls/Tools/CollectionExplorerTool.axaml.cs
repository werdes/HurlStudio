using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.EventArgs;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Tools;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using HurlStudio.Utility;
namespace HurlStudio.UI.Controls.Tools
{
    public partial class CollectionExplorerTool : ViewModelBasedControl<CollectionExplorerToolViewModel>
    {
        private ILogger _log;
        private EditorViewViewModel _editorViewViewModel;
        private CollectionExplorerToolViewModel? _viewModel;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private ICollectionService _collectionService;

        public CollectionExplorerTool(ILogger<CollectionExplorerTool> logger, EditorViewViewModel editorViewViewModel, IEditorService editorService, ControlLocator controlLocator, INotificationService notificationService, ICollectionService collectionService)
        {
            this.InitializeComponent();
            _log = logger;
            _editorViewViewModel = editorViewViewModel;
            _editorService = editorService;
            _notificationService = notificationService;
            _collectionService = collectionService;

            _editorViewViewModel.Collections.CollectionChanged += this.On_Collections_CollectionChanged;
        }

        /// <summary>
        /// On Collection change -> bind new items to local handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Collections_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (HurlCollectionContainer collection in e.NewItems)
                {
                    this.BindCollectionEvents(collection);
                }
            }
        }

        /// <summary>
        /// Sets the controls view model 
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(CollectionExplorerToolViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// OnInitialized
        /// Bind the SelectionChanged event to a handler that propagates the event to all other collections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_CollectionExplorerTool_Initialized(object? sender, EventArgs e)
        {
            foreach (HurlCollectionContainer collectionContainer in _editorViewViewModel.Collections)
            {
                this.BindCollectionEvents(collectionContainer);
            }
        }

        /// <summary>
        /// Binds a collections events to local handlers
        /// </summary>
        /// <param name="collectionContainer"></param>
        private void BindCollectionEvents(HurlCollectionContainer collectionContainer)
        {
            collectionContainer.ControlSelectionChanged -= this.On_CollectionContainer_ControlSelectionChanged;
            collectionContainer.CollectionComponentMoved -= this.On_CollectionContainer_CollectionComponentMoved;
            collectionContainer.ControlSelectionChanged += this.On_CollectionContainer_ControlSelectionChanged;
            collectionContainer.CollectionComponentMoved += this.On_CollectionContainer_CollectionComponentMoved;
        }

        /// <summary>
        /// Propagate the unselect event through all collections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_CollectionContainer_ControlSelectionChanged(object? sender, ControlSelectionChangedEventArgs e)
        {
            foreach (HurlCollectionContainer collectionContainer in _editorViewViewModel.Collections)
            {
                collectionContainer.Unselect();
            }
        }

        /// <summary>
        /// Button click that expands all collections and their folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonExpandAll_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                this.SetCollapsedStateForCollections(false);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonExpandAll_Click));
            }
        }

        /// <summary>
        /// Button click that collapses all collections and their folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_ButtonCollapseAll_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                this.SetCollapsedStateForCollections(true);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_ButtonCollapseAll_Click));
            }
        }

        /// <summary>
        /// Sets the collapsed state for all collections
        /// </summary>
        /// <param name="isCollapsed"></param>
        private void SetCollapsedStateForCollections(bool isCollapsed)
        {
            foreach (HurlCollectionContainer collectionContainer in _editorViewViewModel.Collections)
            {
                foreach (HurlFolderContainer folder in collectionContainer.Folders)
                {
                    this.SetCollapsedStateForFolder(isCollapsed, folder);
                }

                collectionContainer.Collapsed = isCollapsed;
            }
        }

        /// <summary>
        /// Recursively sets the collapsed state for folders
        /// </summary>
        /// <param name="isCollapsed"></param>
        /// <param name="folder"></param>
        private void SetCollapsedStateForFolder(bool isCollapsed, HurlFolderContainer folder)
        {
            if (folder.Found)
            {
                folder.Collapsed = isCollapsed;
                foreach (HurlFolderContainer subFolder in folder.Folders)
                {
                    this.SetCollapsedStateForFolder(isCollapsed, subFolder);
                }
            }
        }

        /// <summary>
        /// Handle the movement of collection components via the editor service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_CollectionContainer_CollectionComponentMoved(object? sender, HurlContainerMovedEventArgs e)
        {
            if (_viewModel == null) return; // should not happen, since moving an object requires an object to be present, which
                                            // originates from the view model
            bool moveSuccessful = false;
            string? sourcePath = null;

            try
            {
                _viewModel.IsEnabled = false;

                // File or folder to collection root
                if (e.Target is HurlCollectionContainer collectionContainer)
                {
                    if (e.Source is HurlFolderContainer collectionRootFolder)
                    {
                        sourcePath = collectionRootFolder.AbsoluteLocation;
                        moveSuccessful = await _editorService.MoveFolderToCollectionRoot(collectionRootFolder, collectionContainer);
                    }
                    else if (e.Source is HurlFileContainer collectionRootFile)
                    {
                        sourcePath = collectionRootFile.AbsoluteLocation;
                        moveSuccessful = await _editorService.MoveFileToCollectionRoot(collectionRootFile, collectionContainer);
                    }
                }
                // File to folder
                else if (e.Source is HurlFileContainer collectionFile &&
                         e.Target is HurlFolderContainer collectionFolder &&
                         collectionFile != null &&
                         collectionFolder != null)
                {
                    // Check if the target folder is in a different collection than the source file
                    if (collectionFile.FolderContainer.CollectionContainer.Collection.CollectionFileLocation == collectionFolder.CollectionContainer.Collection.CollectionFileLocation)
                    {
                        sourcePath = collectionFile.AbsoluteLocation;
                        moveSuccessful = await _editorService.MoveFileToFolder(collectionFile, collectionFolder);
                    }
                    else
                    {
                        sourcePath = collectionFile.AbsoluteLocation;
                        moveSuccessful = await _editorService.MoveFileToCollection(collectionFile, collectionFolder, collectionFolder.CollectionContainer);
                    }
                }
                // Folder to folder
                else if (e.Source is HurlFolderContainer collectionFolderChild &&
                         e.Target is HurlFolderContainer collectionFolderParent &&
                         collectionFolderChild != null &&
                         collectionFolderParent != null)
                {
                    // Check if the target folder is in a different collection than the source folder
                    if (collectionFolderChild.CollectionContainer.Collection.CollectionFileLocation == collectionFolderParent.CollectionContainer.Collection.CollectionFileLocation)
                    {
                        sourcePath = collectionFolderChild.AbsoluteLocation;
                        moveSuccessful = await _editorService.MoveFolderToFolder(collectionFolderChild, collectionFolderParent);
                    }
                    else
                    {
                        sourcePath = collectionFolderChild.AbsoluteLocation;
                        moveSuccessful = await _editorService.MoveFolderToCollection(collectionFolderChild, collectionFolderParent, collectionFolderParent.CollectionContainer);
                    }
                }

                if (!moveSuccessful)
                {
                    _notificationService.Notify(Model.Notifications.NotificationType.Error, Localization.Localization.Service_EditorService_Errors_MoveFile_Failed, sourcePath ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_CollectionContainer_CollectionComponentMoved));
                _notificationService.Notify(ex);
            }
            finally
            {
                _viewModel.IsEnabled = true;
            }
        }

        /// <summary>
        /// Refresh the Collections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonRefresh_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            try
            {
                await this.RefreshCollections();
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_CollectionContainer_CollectionComponentMoved));
                _notificationService.Notify(ex);
            }
            finally
            {
                _viewModel.IsEnabled = true;
            }
        }

        /// <summary>
        /// Refreshes the collections and re-binds their events
        /// </summary>
        /// <returns></returns>
        private async Task RefreshCollections()
        {
            if (_viewModel == null) return;

            _viewModel.IsEnabled = false;
            _editorViewViewModel.Collections.CollectionChanged -= this.On_Collections_CollectionChanged;
            _editorViewViewModel.Collections = await _collectionService.GetCollectionContainersAsync();

            // Re-Bind events
            _editorViewViewModel.Collections.CollectionChanged += this.On_Collections_CollectionChanged;
            foreach (HurlCollectionContainer collectionContainer in _editorViewViewModel.Collections)
            {
                this.BindCollectionEvents(collectionContainer);
            }
        }
    }
}
