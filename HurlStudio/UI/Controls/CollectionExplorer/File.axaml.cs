using Avalonia;
using Avalonia.Controls;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.Utility;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UiState;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class File : CollectionExplorerControlBase<HurlFileContainer>
    {
        private HurlFileContainer? _fileContainer;
        
        private IEditorService _editorService;
        private ILogger _log;
        private INotificationService _notificationService;
        private IUiStateService _uiStateService;
        private MainWindow _mainWindow;

        public File(ILogger<File> logger, INotificationService notificationService, IEditorService editorService, IUiStateService uiStateService, MainWindow mainWindow)
            : base(notificationService, logger)
        {
            _editorService = editorService;
            _log = logger;
            _notificationService = notificationService;
            _uiStateService = uiStateService;
            _mainWindow = mainWindow;

            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void SetViewModelInstance(HurlFileContainer viewModel)
        {
            _fileContainer = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_File_Initialized(object? sender, EventArgs e)
        {
        }

        /// <summary>
        /// Returns the file as bound element
        /// </summary>
        /// <returns></returns>
        protected override HurlContainerBase? GetBoundCollectionComponent()
        {
            return _fileContainer;
        }

        /// <summary>
        /// Opens the file document
        /// </summary>
        /// <returns></returns>
        protected override async Task OpenComponentDocument()
        {
            if (_fileContainer == null) return;
            if (_fileContainer.CollectionContainer.Collection.CollectionFileLocation == null) return;

            try
            {
                await _editorService.OpenFileDocument(_fileContainer.AbsoluteLocation, _fileContainer.CollectionContainer.Collection.CollectionFileLocation);
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
            if (_fileContainer == null) return;

            try
            {
                OSUtility.RevealPathInExplorer(_fileContainer.AbsoluteLocation);
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
            if (_fileContainer == null) return;

            try
            {
                string? inputResult = await MessageBox.MessageBox.AskInputDialog(
                    _mainWindow,
                    Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_Rename_Message,
                    Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_Rename_Title,
                    Path.GetFileName(_fileContainer.AbsoluteLocation),
                    Model.Enums.Icon.Rename);

                if(inputResult != null)
                {
                    await _editorService.RenameFile(_fileContainer, inputResult);
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Opens the file document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Open_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_fileContainer == null) return;

            try
            {
                await this.OpenComponentDocument();
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_fileContainer == null) return;

            try
            {
                bool delete = await MessageBox.MessageBox.ShowQuestionYesNoDialog(
                    _mainWindow, _fileContainer.AbsoluteLocation, Localization.Localization.Dock_Tool_CollectionExplorer_File_MessageBox_DeleteFile_Delete) == MessageBox.MessageBoxResult.Yes;
                if (!delete) return;

                bool deleted = await _editorService.DeleteFile(_fileContainer, false);
                if(!deleted)
                {
                    bool deletePermanently = await MessageBox.MessageBox.ShowQuestionYesNoDialog(
                        _mainWindow, _fileContainer.AbsoluteLocation, Localization.Localization.Dock_Tool_CollectionExplorer_File_MessageBox_DeleteFile_DeletePermanently) == MessageBox.MessageBoxResult.Yes;
                    if(deletePermanently)
                    {
                        deleted = await _editorService.DeleteFile(_fileContainer, true);
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
