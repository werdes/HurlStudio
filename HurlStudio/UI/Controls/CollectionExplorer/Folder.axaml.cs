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
using HurlStudio.Utility;
using System.IO;
using HurlStudio.UI.Windows;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Folder : CollectionExplorerControlBase<HurlFolderContainer>
    {
        private HurlFolderContainer? _folderContainer;
        
        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private IUiStateService _uiStateService;
        private MainWindow _mainWindow;

        public Folder(ILogger<Folder> logger, INotificationService notificationService, IEditorService editorService, IUiStateService uiStateService, MainWindow mainWindow)
            : base(notificationService, logger)
        {
            _log = logger;
            _editorService = editorService;
            _notificationService = notificationService;
            _uiStateService = uiStateService;
            _mainWindow = mainWindow;

            this.InitializeComponent();

        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(HurlFolderContainer viewModel)
        {
            _folderContainer = viewModel;
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
            if (_folderContainer == null) return;

            try
            {
                _folderContainer.Collapsed = !_folderContainer.Collapsed;
                _uiStateService.SetCollectionExplorerCollapseState(_folderContainer.GetId(), _folderContainer.Collapsed);
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
            return _folderContainer;
        }

        /// <summary>
        /// Opens a folder settings page
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            if (_folderContainer == null) return;

            try
            {
                await _editorService.OpenFolder(_folderContainer.AbsoluteLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Opens the folder containing the folder location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_MenuItem_RevealInExplorer_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_folderContainer == null) return;
            if (_folderContainer.AbsoluteLocation == null) return;

            try
            {
                OSUtility.RevealPathInExplorer(_folderContainer.AbsoluteLocation);
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
            if (_folderContainer == null) return;

            try
            {
                await _editorService.OpenFolder(_folderContainer.AbsoluteLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Opens a rename-dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Rename_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_folderContainer == null) return;
            if (_folderContainer.AbsoluteLocation == null) return;
            try
            {
                string? inputResult = await MessageBox.AskInputDialog(
                    _mainWindow,
                    Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_Rename_Message,
                    Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_Rename_Title,
                    new DirectoryInfo(_folderContainer.AbsoluteLocation).Name,
                    Model.Enums.Icon.Rename50);

                if (inputResult != null)
                {
                    await _editorService.RenameFolder(_folderContainer, inputResult);
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Deletes a folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_folderContainer == null) return;

            try
            {
                bool delete = await MessageBox.ShowQuestionYesNoDialog(
                    _mainWindow, _folderContainer.AbsoluteLocation, Localization.Localization.Dock_Tool_CollectionExplorer_Folder_MessageBox_DeleteFolder_Delete) == MessageBox.ButtonType.Yes;
                if (!delete) return;

                bool deleted = await _editorService.DeleteFolder(_folderContainer, false);
                if (!deleted)
                {
                    bool deletePermanently = await MessageBox.ShowQuestionYesNoDialog(
                        _mainWindow, _folderContainer.AbsoluteLocation, Localization.Localization.Dock_Tool_CollectionExplorer_Folder_MessageBox_DeleteFolder_DeletePermanently) == MessageBox.ButtonType.Yes;
                    if (deletePermanently)
                    {
                        deleted = await _editorService.DeleteFolder(_folderContainer, true);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}
