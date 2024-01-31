using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.EventArgs;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Tools;
using Microsoft.Extensions.Logging;
using System;
namespace HurlStudio.UI.Controls.Tools
{
    public partial class CollectionExplorerTool : ViewModelBasedControl<CollectionExplorerToolViewModel>
    {
        private ILogger _log;
        private EditorViewViewModel _editorViewViewModel;
        private CollectionExplorerToolViewModel? _viewModel;
        private IEditorService _editorService;
        private INotificationService _notificationService;

        public CollectionExplorerTool(ILogger<CollectionExplorerTool> logger, EditorViewViewModel editorViewViewModel, IEditorService editorService, ControlLocator controlLocator, INotificationService notificationService)
        {
            InitializeComponent();
            _log = logger;
            _editorViewViewModel = editorViewViewModel;
            _editorService = editorService;
            _notificationService = notificationService;

            _editorViewViewModel.Collections.CollectionChanged += On_Collections_CollectionChanged;
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
                foreach (CollectionContainer collection in e.NewItems)
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
        private void On_CollectionExplorerTool_Initialized(object? sender, System.EventArgs e)
        {
            foreach (CollectionContainer collectionContainer in _editorViewViewModel.Collections)
            {
                BindCollectionEvents(collectionContainer);
            }
        }

        /// <summary>
        /// Binds a collections events to local handlers
        /// </summary>
        /// <param name="collectionContainer"></param>
        private void BindCollectionEvents(CollectionContainer collectionContainer)
        {
            collectionContainer.ControlSelectionChanged += On_CollectionContainer_ControlSelectionChanged;
            collectionContainer.CollectionComponentMoved += On_CollectionContainer_CollectionComponentMoved;
        }

        /// <summary>
        /// Propagate the unselect event through all collections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_CollectionContainer_ControlSelectionChanged(object? sender, ControlSelectionChangedEventArgs e)
        {
            foreach (CollectionContainer collectionContainer in _editorViewViewModel.Collections)
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
                SetCollapsedStateForCollections(false);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_ButtonExpandAll_Click));
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
                SetCollapsedStateForCollections(true);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_ButtonCollapseAll_Click));
            }
        }

        /// <summary>
        /// Sets the collapsed state for all collections
        /// </summary>
        /// <param name="isCollapsed"></param>
        private void SetCollapsedStateForCollections(bool isCollapsed)
        {
            foreach (CollectionContainer collectionContainer in _editorViewViewModel.Collections)
            {
                foreach (CollectionFolder folder in collectionContainer.Folders)
                {
                    SetCollapsedStateForFolder(isCollapsed, folder);
                }

                collectionContainer.Collapsed = isCollapsed;
            }
        }

        /// <summary>
        /// Recursively sets the collapsed state for folders
        /// </summary>
        /// <param name="isCollapsed"></param>
        /// <param name="folder"></param>
        private void SetCollapsedStateForFolder(bool isCollapsed, CollectionFolder folder)
        {
            if (folder.Found)
            {
                folder.Collapsed = isCollapsed;
                foreach (CollectionFolder subFolder in folder.Folders)
                {
                    SetCollapsedStateForFolder(isCollapsed, subFolder);
                }
            }
        }

        /// <summary>
        /// Handle the movement of collection components via the editor service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_CollectionContainer_CollectionComponentMoved(object? sender, CollectionComponentMovedEventArgs e)
        {
            if (_viewModel == null) return; // should not happen, since moving an object requires an object to be present, which
                                            // originates from the view model
            try
            {
                _viewModel.IsEnabled = false;

                // File or folder to collection root
                if (e.Target is CollectionContainer collectionContainer)
                {
                    if (e.Source is CollectionFolder collectionRootFolder)
                    {
                        await _editorService.MoveFolderToCollectionRoot(collectionRootFolder, collectionContainer);
                    }
                    else if (e.Source is CollectionFile collectionRootFile)
                    {
                        await _editorService.MoveFileToCollectionRoot(collectionRootFile, collectionContainer);
                    }
                }
                // File to folder
                else if (e.Source is CollectionFile collectionFile &&
                         e.Target is CollectionFolder collectionFolder &&
                         collectionFile != null &&
                         collectionFolder != null)
                {
                    // Check if the target folder is in a different collection than the source file
                    if (collectionFile.Folder.CollectionContainer == collectionFolder.CollectionContainer)
                    {
                        await _editorService.MoveFileToFolder(collectionFile, collectionFolder);
                    }
                    else
                    {
                        await _editorService.MoveFileToCollection(collectionFile, collectionFolder, collectionFolder.CollectionContainer);
                    }
                }
                // Folder to folder
                else if (e.Source is CollectionFolder collectionFolderChild &&
                         e.Target is CollectionFolder collectionFolderParent &&
                         collectionFolderChild != null &&
                         collectionFolderParent != null)
                {
                    // Check if the target folder is in a different collection than the source folder
                    if (collectionFolderChild.CollectionContainer == collectionFolderParent.CollectionContainer)
                    {
                        await _editorService.MoveFolderToFolder(collectionFolderChild, collectionFolderParent);
                    }
                    else
                    {
                        await _editorService.MoveFolderToCollection(collectionFolderChild, collectionFolderParent, collectionFolderParent.CollectionContainer);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(On_CollectionContainer_CollectionComponentMoved));
                _notificationService.Notify(ex);
            }
            finally
            {
                _viewModel.IsEnabled = true;
            }
        }
    }
}
