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
using System.IO;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using HurlStudio.Model.Notifications;
using HurlStudio.Model.HurlFileTemplates;

namespace HurlStudio.UI.Controls.CollectionExplorer
{
    public partial class Collection : CollectionExplorerControlBase<HurlCollectionContainer>
    {
        private HurlCollectionContainer? _collectionContainer;

        private ILogger _log;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private IUiStateService _uiStateService;
        private MainWindow _mainWindow;
        private ServiceManager<Windows.WindowBase> _windowBuilder;

        public Collection(ILogger<Collection> logger, INotificationService notificationService, IEditorService editorService, IUiStateService uiStateService, MainWindow mainWindow, ServiceManager<Windows.WindowBase> windowBuilder)
            : base(notificationService, logger)
        {
            _editorService = editorService;
            _uiStateService = uiStateService;
            _log = logger;
            _notificationService = notificationService;
            _mainWindow = mainWindow;
            _windowBuilder = windowBuilder;

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
                OSUtility.RevealPathInExplorer(_collectionContainer.Collection.CollectionFileLocation);
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


        /// <summary>
        /// Opens a rename-dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Rename_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_collectionContainer == null) return;

            try
            {
                string? inputResult = await MessageBox.MessageBox.AskInputDialog(
                    _mainWindow,
                    Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_Rename_Message,
                    Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_Rename_Title,
                    _collectionContainer.Collection.Name,
                    Model.Enums.Icon.Rename);

                if (inputResult != null)
                {
                    bool moveFile =
                        await MessageBox.MessageBox.ShowQuestionYesNoDialog( 
                            _mainWindow,
                            Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_RenameCollection_MoveCollectionFile,
                            Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_Rename_Title) == MessageBox.MessageBoxResult.Yes;

                    await _editorService.RenameCollection(_collectionContainer, inputResult, moveFile);
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Removes a collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_Remove_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_collectionContainer == null) return;

            try
            {
                bool remove = await MessageBox.MessageBox.ShowQuestionYesNoDialog(
                    _mainWindow, _collectionContainer.Collection.CollectionFileLocation, Localization.Localization.Dock_Tool_CollectionExplorer_Collection_MessageBox_RemoveCollection) == MessageBox.MessageBoxResult.Yes;
                if (!remove) return;

                bool deleted = await _editorService.RemoveCollection(_collectionContainer);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
        
        /// <summary>
        /// Adds a folder to a collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_AddFolder_Click(object? sender, RoutedEventArgs e)
        {
            if (_collectionContainer == null) return;

            try
            {
                string? folderName = await MessageBox.MessageBox.AskInputDialog(_mainWindow,
                    Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_AddFolder_Message,
                    Localization.Localization.Dock_Tool_CollectionExplorer_MessageBox_AddFolder_Title, string.Empty,
                    Model.Enums.Icon.AddFolder);
                string? collectionDirectory = Path.GetDirectoryName(_collectionContainer.Collection.CollectionFileLocation);

                if (!string.IsNullOrWhiteSpace(folderName) && collectionDirectory != null)
                {
                    string newPath = Path.Combine(collectionDirectory, folderName.GetValidDirectoryName());
                    if (!Directory.Exists(newPath))
                    {
                        await _editorService.CreateFolder(_collectionContainer, newPath);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Adds a file to a collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_MenuItem_AddFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_collectionContainer == null) return;
            if (_window == null) return;

            try
            {
                AddFileWindow addFileWindow = _windowBuilder.Get<AddFileWindow>();
                (string fileName, HurlFileTemplateContainer template) = await addFileWindow.ShowDialog<(string, HurlFileTemplateContainer)>(_window);

                if (fileName == null || template == null) return;
                await _editorService.CreateFileInCollectionRoot(_collectionContainer, template, fileName);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }
    }
}
