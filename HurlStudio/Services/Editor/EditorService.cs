﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Collections.Model;
using HurlStudio.Collections.Settings;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.Common.Utility;
using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Model.UiState;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Dock;
using HurlStudio.UI.Localization;
using HurlStudio.UI.MessageBox;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Documents;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WindowBase = HurlStudio.UI.Windows.WindowBase;

namespace HurlStudio.Services.Editor
{
    public class EditorService : IEditorService
    {
        private readonly ICollectionService _collectionService;
        private readonly IConfiguration _configuration;
        private readonly ServiceManager<Document> _documentControlBuilder;
        private readonly EditorViewViewModel _editorViewViewModel;
        private readonly IEnvironmentService _environmentService;
        private readonly LayoutFactory _layoutFactory;
        private readonly ILogger _log;
        private readonly MainViewViewModel _mainViewViewModel;
        private readonly SemaphoreLock _moveLock = new SemaphoreLock();
        private readonly INotificationService _notificationService;

        private readonly SemaphoreLock _saveLock = new SemaphoreLock();
        private readonly IUiStateService _uiStateService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly ServiceManager<UI.Windows.WindowBase> _windowBuilder;
        private bool _fileHistoryBlocked = false;
        private int _fileHistoryLength = 0;


        public EditorService(ILogger<EditorService> logger, ServiceManager<Document> documentControlBuilder,
            EditorViewViewModel editorViewViewModel, MainViewViewModel mainViewViewModel, LayoutFactory layoutFactory,
            IUserSettingsService userSettingsService, INotificationService notificationService,
            IConfiguration configuration, IUiStateService uiStateService, ICollectionService collectionService,
            IEnvironmentService environmentService, ServiceManager<WindowBase> windowBuilder)
        {
            _editorViewViewModel = editorViewViewModel;
            _mainViewViewModel = mainViewViewModel;

            _log = logger;
            _documentControlBuilder = documentControlBuilder;
            _layoutFactory = layoutFactory;
            _uiStateService = uiStateService;
            _userSettingsService = userSettingsService;
            _notificationService = notificationService;
            _configuration = configuration;
            _collectionService = collectionService;
            _environmentService = environmentService;
            _windowBuilder = windowBuilder;

            _fileHistoryLength = Math.Max(_configuration.GetValue<int>("fileHistoryLength"),
                GlobalConstants.DEFAULT_FILE_HISTORY_LENGTH);

            _editorViewViewModel.ActiveEnvironmentChanged += this.On_EditorViewViewModel_ActiveEnvironmentChanged;
            _layoutFactory.ActiveDockableChanged += this.On_LayoutFactory_ActiveDockableChanged;
        }

        /// <inheritdoc />
        public async Task HistoryGoBack()
        {
            if (_editorViewViewModel.DocumentHistory.Count == 0) return;
            if (_editorViewViewModel.Layout == null) return;

            DocumentBase? historyDocument;
            while ((historyDocument = _editorViewViewModel.DocumentHistory.Pop()) == null)
            {
            }

            _editorViewViewModel.DocumentFuture.Push(_editorViewViewModel.ActiveDocument);
            _fileHistoryBlocked = true;
            _layoutFactory.SetActiveDockable(historyDocument);
            _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, historyDocument);
            _fileHistoryBlocked = false;
        }

        /// <inheritdoc />
        public async Task HistoryGoForward()
        {
            if (_editorViewViewModel.DocumentFuture.Count == 0) return;
            if (_editorViewViewModel.Layout == null) return;

            DocumentBase? historyDocument;
            while ((historyDocument = _editorViewViewModel.DocumentFuture.Pop()) == null)
            {
            }

            _editorViewViewModel.DocumentHistory.Push(historyDocument);
            _fileHistoryBlocked = true;
            _layoutFactory.SetActiveDockable(historyDocument);
            _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, historyDocument);
            _fileHistoryBlocked = false;
        }

        /// <inheritdoc />
        public async Task OpenInitialDocuments()
        {
            _layoutFactory.AddDocument(_documentControlBuilder.Get<WelcomeDocumentViewModel>());

            // Open files from previous session
            Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(false);
            if (uiState != null)
            {
                foreach (string documentPath in uiState.OpenedDocuments)
                {
                    await this.OpenPathDocument(documentPath);
                }

                // Open the active document tab
                if (uiState.ActiveDocument != null)
                {
                    await this.OpenPathDocument(uiState.ActiveDocument);
                    _editorViewViewModel.ActiveDocument =
                        (DocumentBase?)_editorViewViewModel.DocumentDock?.ActiveDockable;
                }
            }
        }

        /// <inheritdoc />
        public async Task OpenPathDocument(string documentPath)
        {
            if (Directory.Exists(documentPath))
            {
                await this.OpenFolderDocument(documentPath);
            }
            else if (Path.GetExtension(documentPath).ToLower() == GlobalConstants.HURL_FILE_EXTENSION)
            {
                await this.OpenFileDocument(documentPath);
            }
            else if (Path.GetExtension(documentPath).ToLower() == GlobalConstants.COLLECTION_FILE_EXTENSION)
            {
                await this.OpenCollectionDocument(documentPath);
            }
            else if (Path.GetExtension(documentPath).ToLower() == GlobalConstants.ENVIRONMENT_FILE_EXTENSION)
            {
                await this.OpenEnvironmentDocument(documentPath);
            }
        }

        /// <inheritdoc />
        public async Task AddCollection()
        {
            if (_editorViewViewModel.View?.Window == null) return;

            // Ask User for file path
            ReadOnlyCollection<FilePickerFileType> fileTypeFilters = new ReadOnlyCollection<FilePickerFileType>(
                new List<FilePickerFileType>()
                {
                    new FilePickerFileType(Localization
                        .Service_EditorService_FilePicker_CollectionFile_Filter)
                    {
                        Patterns = [$"*{GlobalConstants.COLLECTION_FILE_EXTENSION}"]
                    }
                });
            string? collectionFilePath = await StorageUtility.DisplayOpenFilePickerSingle(
                _editorViewViewModel.View?.Window.StorageProvider,
                Localization.Service_EditorService_FilePicker_CollectionFile_Title,
                fileTypeFilters.ToArray());
            if (string.IsNullOrEmpty(collectionFilePath) ||
                !File.Exists(collectionFilePath))
                return;

            Model.UserSettings.UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);

            if (userSettings.CollectionFiles == null ||
                userSettings.CollectionFiles.Any(x => x.ToNormalized() == collectionFilePath.ToNormalized()))
                return;

            userSettings.CollectionFiles.Add(collectionFilePath);
            await _userSettingsService.StoreUserSettingsAsync();
            await this.RefreshCollectionExplorerCollections();
        }

        /// <inheritdoc />
        public async Task LoadInitialUserSettings()
        {
            Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(false);
            Model.UserSettings.UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);

            _editorViewViewModel.FileHistoryEntries.AddRangeIfNotNull(uiState?.FileHistoryEntries);
            _editorViewViewModel.ShowWhitespace = userSettings.ShowWhitespace;
            _editorViewViewModel.ShowEndOfLine = userSettings.ShowEndOfLine;
            _editorViewViewModel.WordWrap = userSettings.WordWrap;
        }

        /// <summary>
        /// Move a file from one collection to another
        /// -> Remove the file from its previous collection
        /// -> Add file settings to its target collection
        /// -> Physically move the .hurl file to the target folder
        /// -> Refresh collection
        /// </summary>
        /// <param name="originalFileContainer"></param>
        /// <param name="newParentFolderContainer"></param>
        /// <param name="newCollectionContainer"></param>
        /// <returns></returns>
        public async Task<bool> MoveFileToCollection(HurlFileContainer originalFileContainer,
            HurlFolderContainer newParentFolderContainer, HurlCollectionContainer newCollectionContainer)
        {
            if (_editorViewViewModel.Layout == null) return false;

            _log.LogInformation(
                $"Moving [{originalFileContainer}] to [{newCollectionContainer}], folder [{newParentFolderContainer}]");

            return await _moveLock.LockAsync<bool>(async () =>
            {
                // Move file and reopen, if successful
                string newPath = Path.Combine(newParentFolderContainer.AbsoluteLocation,
                    Path.GetFileName(originalFileContainer.AbsoluteLocation));
                (bool moveSuccessful, bool reopenFile) = await this.MoveFile(
                    originalFileContainer,
                    originalFileContainer.CollectionContainer.Collection.CollectionFileLocation,
                    newCollectionContainer.Collection.CollectionFileLocation,
                    newPath);

                if (moveSuccessful)
                {
                    // Refresh collections
                    await this.RefreshCollectionExplorerCollection(originalFileContainer.CollectionContainer.Collection
                        .CollectionFileLocation);
                    await this.RefreshCollectionExplorerCollection(newCollectionContainer.Collection
                        .CollectionFileLocation);

                    // Re-Open the file at the new location
                    if (reopenFile)
                    {
                        await this.OpenFileDocument(newPath);
                    }

                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Move a file from one collection to another, into the root path
        /// -> Remove the file from its previous collection
        /// -> Add file settings to its target collection
        /// -> Physically move the .hurl file to the root folder of the target collection
        /// -> Refresh collections
        /// </summary>
        /// <param name="originalFileContainer"></param>
        /// <param name="newCollectionContainer"></param>
        /// <returns></returns>
        public async Task<bool> MoveFileToCollectionRoot(HurlFileContainer originalFileContainer,
            HurlCollectionContainer newCollectionContainer)
        {
            if (_editorViewViewModel.Layout == null) return false;

            _log.LogInformation($"Moving [{originalFileContainer}] to [{newCollectionContainer}] root folder");

            return await _moveLock.LockAsync<bool>(async () =>
            {
                // Move file and reopen, if successful
                string? collectionDirectory =
                    Path.GetDirectoryName(newCollectionContainer.Collection.CollectionFileLocation);
                if (collectionDirectory == null) return false;

                string newPath = Path.Combine(collectionDirectory,
                    Path.GetFileName(originalFileContainer.AbsoluteLocation));
                (bool moveSuccessful, bool reopenFile) = await this.MoveFile(
                    originalFileContainer,
                    originalFileContainer.CollectionContainer.Collection.CollectionFileLocation,
                    newCollectionContainer.Collection.CollectionFileLocation,
                    newPath);

                if (!moveSuccessful) return false;

                // Refresh collections
                await this.RefreshCollectionExplorerCollection(originalFileContainer.CollectionContainer.Collection
                    .CollectionFileLocation);
                await this.RefreshCollectionExplorerCollection(newCollectionContainer.Collection
                    .CollectionFileLocation);

                // Re-Open the file at the new location
                if (reopenFile)
                {
                    await this.OpenFileDocument(newPath);
                }

                return true;
            });
        }

        /// <summary>
        /// Move a file to a folder in the same collection
        /// </summary>
        /// <param name="originalFileContainer"></param>
        /// <param name="targetFolderContainer"></param>
        /// <returns></returns>
        public async Task<bool> MoveFileToFolder(HurlFileContainer originalFileContainer,
            HurlFolderContainer targetFolderContainer)
        {
            if (_editorViewViewModel.Layout == null) return false;

            _log.LogInformation($"Moving [{originalFileContainer}] to [{targetFolderContainer}]");

            return await _moveLock.LockAsync<bool>(async () =>
            {
                // Move file and reopen, if successful
                string newPath = Path.Combine(targetFolderContainer.AbsoluteLocation,
                    Path.GetFileName(originalFileContainer.AbsoluteLocation));
                (bool moveSuccessful, bool reopenFile) = await this.MoveFile(
                    originalFileContainer,
                    originalFileContainer.CollectionContainer.Collection.CollectionFileLocation,
                    originalFileContainer.CollectionContainer.Collection.CollectionFileLocation,
                    newPath);

                if (moveSuccessful)
                {
                    // Refresh collections
                    await this.RefreshCollectionExplorerCollection(originalFileContainer.CollectionContainer.Collection
                        .CollectionFileLocation);

                    // Re-Open the file at the new location
                    if (reopenFile)
                    {
                        await this.OpenFileDocument(newPath);
                    }

                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Move a folder to a different collection
        /// -> Create folder in target collection
        /// -> move all files and subfolders to new collection recursively
        /// -> remove old folder
        /// </summary>
        /// <param name="originalFolderContainer"></param>
        /// <param name="newParentFolderContainer"></param>
        /// <param name="newCollectionContainer"></param>
        /// <returns></returns>
        public async Task<bool> MoveFolderToCollection(HurlFolderContainer originalFolderContainer,
            HurlFolderContainer newParentFolderContainer, HurlCollectionContainer newCollectionContainer)
        {
            if (_editorViewViewModel.Layout == null) return false;
            List<string> reopenPaths;

            _log.LogInformation(
                $"Moving [{originalFolderContainer}] to [{newCollectionContainer}], folder [{newParentFolderContainer}]");


            return await _moveLock.LockAsync<bool>(async () =>
            {
                // Move folder and reopen, if successful
                string folderName = new DirectoryInfo(originalFolderContainer.AbsoluteLocation).Name;
                string newPath = Path.Combine(newParentFolderContainer.AbsoluteLocation, folderName);
                (bool moveSuccessful, reopenPaths) = await this.MoveFolder(
                    originalFolderContainer,
                    originalFolderContainer.CollectionContainer.Collection.CollectionFileLocation,
                    newCollectionContainer.Collection.CollectionFileLocation,
                    newPath);

                if (moveSuccessful)
                {
                    // Refresh collections
                    await this.RefreshCollectionExplorerCollection(originalFolderContainer.CollectionContainer
                        .Collection.CollectionFileLocation);
                    await this.RefreshCollectionExplorerCollection(newCollectionContainer.Collection
                        .CollectionFileLocation);

                    // Re-Open the file at the new location
                    foreach (string reopenPath in reopenPaths)
                    {
                        await this.OpenPathDocument(reopenPath);
                    }

                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Moves a folder to any collections' root folder
        /// </summary>
        /// <param name="originalFolderContainer"></param>
        /// <param name="newCollectionContainer"></param>
        /// <returns></returns>
        public async Task<bool> MoveFolderToCollectionRoot(HurlFolderContainer originalFolderContainer,
            HurlCollectionContainer newCollectionContainer)
        {
            if (_editorViewViewModel.Layout == null) return false;
            List<string> reopenPaths;

            _log.LogInformation($"Moving [{originalFolderContainer}] to [{newCollectionContainer}] root directory");

            return await _moveLock.LockAsync<bool>(async () =>
            {
                // Move folder and reopen, if successful
                string? collectionDirectory =
                    Path.GetDirectoryName(newCollectionContainer.Collection.CollectionFileLocation);
                if (collectionDirectory == null) return false;

                string folderName = new DirectoryInfo(originalFolderContainer.AbsoluteLocation).Name;
                string newPath = Path.Combine(collectionDirectory, folderName);

                (bool moveSuccessful, reopenPaths) = await this.MoveFolder(
                    originalFolderContainer,
                    originalFolderContainer.CollectionContainer.Collection.CollectionFileLocation,
                    newCollectionContainer.Collection.CollectionFileLocation,
                    newPath);

                if (moveSuccessful)
                {
                    // Refresh collections
                    await this.RefreshCollectionExplorerCollection(originalFolderContainer.CollectionContainer
                        .Collection.CollectionFileLocation);
                    await this.RefreshCollectionExplorerCollection(newCollectionContainer.Collection
                        .CollectionFileLocation);

                    // Re-Open the file at the new location
                    foreach (string reopenPath in reopenPaths)
                    {
                        await this.OpenPathDocument(reopenPath);
                    }

                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Moves a folder to another folder in the same collection
        /// </summary>
        /// <param name="originalFolderContainer"></param>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        public async Task<bool> MoveFolderToFolder(HurlFolderContainer originalFolderContainer,
            HurlFolderContainer parentFolder)
        {
            if (_editorViewViewModel.Layout == null) return false;
            List<string> reopenPaths;

            _log.LogInformation($"Moving [{originalFolderContainer}] to [{parentFolder}]");

            return await _moveLock.LockAsync<bool>(async () =>
            {
                // Move folder and reopen, if successful
                string folderName = new DirectoryInfo(originalFolderContainer.AbsoluteLocation).Name;
                string newPath = Path.Combine(parentFolder.AbsoluteLocation, folderName);

                (bool moveSuccessful, reopenPaths) = await this.MoveFolder(
                    originalFolderContainer,
                    originalFolderContainer.CollectionContainer.Collection.CollectionFileLocation,
                    originalFolderContainer.CollectionContainer.Collection.CollectionFileLocation,
                    newPath);

                if (moveSuccessful)
                {
                    // Refresh collections
                    await this.RefreshCollectionExplorerCollection(originalFolderContainer.CollectionContainer
                        .Collection.CollectionFileLocation);


                    // Re-Open the file at the new location
                    foreach (string reopenPath in reopenPaths)
                    {
                        await this.OpenPathDocument(reopenPath);
                    }

                    return true;
                }

                return false;
            });
        }


        /// <inheritdoc />
        public async Task<bool> RenameFile(HurlFileContainer fileContainer)
        {
            if (_editorViewViewModel.Layout == null) return false;
            if (_editorViewViewModel.View?.Window == null) return false;

            string? newFileName = await MessageBox.AskInputDialog(
                _editorViewViewModel.View.Window,
                Localization.Service_EditorService_Rename_File_Message,
                Localization.Service_EditorService_Rename_File_Title,
                Path.GetFileName(fileContainer.AbsoluteLocation),
                Icon.Rename);

            if (newFileName == null) return false;
            _log.LogInformation($"Renaming [{fileContainer}] to [{newFileName}]");

            return await _moveLock.LockAsync<bool>(async () =>
            {
                // Move file and reopen, if successful
                string? fileDirectory = Path.GetDirectoryName(fileContainer.AbsoluteLocation);
                if (fileDirectory == null) return false;

                string newPath = Path.Combine(fileDirectory, newFileName);
                (bool moveSuccessful, bool reopenFile) = await this.MoveFile(
                    fileContainer,
                    fileContainer.CollectionContainer.Collection.CollectionFileLocation,
                    fileContainer.CollectionContainer.Collection.CollectionFileLocation,
                    newPath);

                if (moveSuccessful)
                {
                    // Refresh collections
                    await this.RefreshCollectionExplorerCollection(fileContainer.CollectionContainer.Collection
                        .CollectionFileLocation);

                    // Re-Open the file at the new location
                    if (reopenFile)
                    {
                        await this.OpenFileDocument(newPath);
                    }

                    return true;
                }

                return false;
            });
        }

        /// <inheritdoc />
        public async Task<bool> RenameFolder(HurlFolderContainer folderContainer)
        {
            if (_editorViewViewModel.Layout == null) return false;
            if (_editorViewViewModel.View?.Window == null) return false;
            List<string> reopenPaths;

            string? newFolderName = await MessageBox.AskInputDialog(
                _editorViewViewModel.View.Window,
                Localization.Service_EditorService_Rename_Folder_Message,
                Localization.Service_EditorService_Rename_Folder_Title,
                Path.GetFileName(folderContainer.AbsoluteLocation),
                Icon.Rename);

            if (newFolderName == null) return false;
            _log.LogInformation($"Renaming [{folderContainer}] to [{newFolderName}]");

            // Move file and reopen, if successful
            string? folderDirectory = Path.GetDirectoryName(folderContainer.AbsoluteLocation);
            if (folderDirectory == null) return false;

            string newPath = Path.Combine(folderDirectory, newFolderName);

            (bool moveSuccessful, reopenPaths) = await this.MoveFolder(
                folderContainer,
                folderContainer.CollectionContainer.Collection.CollectionFileLocation,
                folderContainer.CollectionContainer.Collection.CollectionFileLocation,
                newPath);

            if (!moveSuccessful) return false;

            // Refresh collections
            await this.RefreshCollectionExplorerCollection(folderContainer.CollectionContainer.Collection
                .CollectionFileLocation);

            // Re-Open the file at the new location
            foreach (string reopenPath in reopenPaths)
            {
                await this.OpenPathDocument(reopenPath);
            }

            return true;
        }


        /// <inheritdoc />
        public async Task<bool> RenameCollection(HurlCollectionContainer collectionContainer)
        {
            if (_editorViewViewModel.View?.Window == null) return false;
            if (_editorViewViewModel.Layout == null) return false;
            if (_editorViewViewModel.DocumentDock?.VisibleDockables == null) return false;

            Model.UserSettings.UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);
            bool reopenFile = false;
            bool moveFile = false;

            string? newCollectionName = await MessageBox.AskInputDialog(
                _editorViewViewModel.View.Window,
                Localization.Service_EditorService_Rename_Collection_Message,
                Localization.Service_EditorService_Rename_Collection_Title,
                collectionContainer.Collection.Name,
                Icon.Rename);
            if (newCollectionName == null || newCollectionName == collectionContainer.Collection.Name) return false;

            moveFile = await MessageBox.ShowQuestionYesNoDialog(
                           _editorViewViewModel.View.Window,
                           Localization.Service_EditorService_Rename_Collection_MoveCollectionFile,
                           Localization.Service_EditorService_Rename_Collection_Title) ==
                       MessageBoxResult.Yes;

            _log.LogInformation($"Renaming [{collectionContainer}] to [{newCollectionName}]");
            string originalFilePath = collectionContainer.Collection.CollectionFileLocation;

            // Close file if necessary
            CollectionDocumentViewModel? openedCollectionDocument =
                this.GetCollectionDocumentByAbsoluteCollectionLocation(originalFilePath);
            if (openedCollectionDocument != null)
            {
                if (!await _layoutFactory.CloseDockableAsync(openedCollectionDocument))
                {
                    return false;
                }

                reopenFile = true;
            }

            string? collectionDirectory = Path.GetDirectoryName(originalFilePath);
            if (collectionDirectory == null) return false;

            string newFilePath = Path.Combine(collectionDirectory,
                Path.ChangeExtension(newCollectionName, GlobalConstants.COLLECTION_FILE_EXTENSION));

            bool renamedSuccessfully =
                await _collectionService.RenameCollection(collectionContainer, newCollectionName, moveFile, newFilePath);
            if (!renamedSuccessfully) return false;

            // If the file was moved, we will need to update the usersettings' collection list
            if (moveFile)
            {
                // Change file name in user settings
                userSettings.CollectionFiles?.Remove(originalFilePath);
                userSettings.CollectionFiles?.Add(newFilePath);
                await _userSettingsService.StoreUserSettingsAsync();

                // Reopen all affected documents
                IEditorDocument? activeEditorDocument = _editorViewViewModel.ActiveDocument as IEditorDocument;
                string? activeDocument = activeEditorDocument?.HurlContainer?.GetPath();
                List<string> reopenDocuments = new List<string>();
                List<IDockable> closeDocuments = new List<IDockable>();

                foreach (IDockable dockable in _editorViewViewModel.DocumentDock.VisibleDockables)
                {
                    if (dockable is IEditorDocument editorDocument &&
                        editorDocument.HurlContainer != null &&
                        editorDocument.UnderlyingCollection?.Collection.CollectionFileLocation == originalFilePath)
                    {
                        reopenDocuments.Add(editorDocument.HurlContainer.GetPath());
                        closeDocuments.Add(dockable);
                    }
                }

                // Close the documents
                foreach (IDockable dockable in closeDocuments)
                {
                    await _layoutFactory.CloseDockableAsync(dockable);
                }

                // Reopen all documents and set the previous one active
                foreach (string reopenDocument in reopenDocuments)
                {
                    await this.OpenPathDocument(reopenDocument);
                }

                IDockable? newActiveDocument = _editorViewViewModel.DocumentDock.VisibleDockables.FirstOrDefault(
                    x => x is IEditorDocument document && document.HurlContainer?.GetPath() == activeDocument);
                if (newActiveDocument != null)
                {
                    _layoutFactory.SetActiveDockable(newActiveDocument);
                }
            }

            // Re-Open the collection at the new location
            if (reopenFile)
            {
                await this.OpenFileDocument(newFilePath);
            }

            await this.RefreshCollectionExplorerCollections();

            return true;
        }

        /// <inheritdoc />
        public async Task OpenFileDocument(string fileLocation, string collectionLocation)
        {
            if (_editorViewViewModel.Layout == null)
                throw new ArgumentNullException(nameof(_editorViewViewModel.Layout));

            try
            {
                _log.LogInformation($"Opening file document for [{fileLocation}]");
                FileDocumentViewModel? openDocument = this.GetFileDocumentByAbsoluteFilePath(fileLocation);

                // Document isn't opened in the dock already
                if (openDocument == null)
                {
                    // Open collection 
                    if (!File.Exists(fileLocation)) throw new IOException($"{fileLocation} does not exist");
                    if (!File.Exists(collectionLocation)) throw new IOException($"{collectionLocation} does not exist");

                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.OpeningFile;
                    _mainViewViewModel.StatusBarDetail = fileLocation;

                    FileDocumentViewModel fileDocument = _documentControlBuilder.Get<FileDocumentViewModel>();
                    fileDocument.SettingAdded += this.On_IEditorDocument_SettingAdded;
                    fileDocument.SettingRemoved += this.On_IEditorDocument_SettingRemoved;

                    await this.SetFileDocument(fileDocument, fileLocation, collectionLocation);

                    // Open in dock
                    _layoutFactory.AddDocument(fileDocument);
                    _layoutFactory.SetActiveDockable(fileDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, fileDocument);

                    //Close welcome document if configured
                    await this.CloseWelcomeDocument();

                    // Place fileDocument in history
                    if (fileDocument.FileContainer != null)
                    {
                        _editorViewViewModel.FileHistoryEntries.RemoveAll(x =>
                            x.Location == fileDocument.FileContainer.AbsoluteLocation);
                        _editorViewViewModel.FileHistoryEntries.Add(
                            new FileHistoryEntry(fileDocument.FileContainer.AbsoluteLocation, DateTime.UtcNow));
                        _editorViewViewModel.FileHistoryEntries = new ObservableCollection<FileHistoryEntry>(
                            _editorViewViewModel.FileHistoryEntries.OrderByDescending(x => x.LastOpened)
                                .Take(_fileHistoryLength));
                    }
                }
                else
                {
                    _layoutFactory.SetActiveDockable(openDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, openDocument);
                    _log.LogInformation($"Document for [{fileLocation}] is already opened");
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);

                throw new Exception($"Opening file document [{fileLocation}] failed", ex);
            }
            finally
            {
                _mainViewViewModel.StatusBarStatus = StatusBarStatus.Idle;
                _mainViewViewModel.StatusBarDetail = string.Empty;
            }
        }

        /// <inheritdoc />
        public async Task OpenFileDocument(string fileLocation)
        {
            HurlFileContainer? collectionFile = _editorViewViewModel.Collections
                .SelectMany(x => this.GetAllFilesFromCollection(x)).Where(x => x.AbsoluteLocation == fileLocation)
                .FirstOrDefault();

            if (collectionFile != null &&
                collectionFile.CollectionContainer != null &&
                collectionFile.CollectionContainer.Collection != null &&
                collectionFile.CollectionContainer.Collection.CollectionFileLocation != null)
            {
                await this.OpenFileDocument(fileLocation,
                    collectionFile.CollectionContainer.Collection.CollectionFileLocation);
            }
        }

        /// <inheritdoc />
        public async Task<bool> CloseFileDocument(FileDocumentViewModel? fileDocument)
        {
            if (fileDocument == null) return true;
            List<HurlSettingContainer> allSettingContainers =
                fileDocument.SettingSections.SelectMany(x => x.SettingContainers).ToList();

            foreach (HurlSettingContainer hurlSettingContainer in allSettingContainers)
            {
                hurlSettingContainer.SettingEnabledChanged -= this.On_HurlSettingContainer_SettingEnabledChanged;
                hurlSettingContainer.SettingOrderChanged -= this.On_HurlSettingContainer_SettingOrderChanged;
                hurlSettingContainer.SettingKeyChanged -= this.On_HurlSettingContainer_SettingKeyChanged;
            }

            return true;
        }

        /// <inheritdoc />
        public async Task OpenFolderDocument(string folderLocation, string collectionLocation)
        {
            if (_editorViewViewModel.Layout == null)
                throw new ArgumentNullException(nameof(_editorViewViewModel.Layout));

            try
            {
                _log.LogInformation($"Opening folder document for [{folderLocation}]");
                FolderDocumentViewModel? openDocument = this.GetFolderDocumentByAbsoluteFolderLocation(folderLocation);

                // Document isn't opened in the dock already
                if (openDocument == null)
                {
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.OpeningFile;
                    _mainViewViewModel.StatusBarDetail = folderLocation;


                    FolderDocumentViewModel folderDocument = _documentControlBuilder.Get<FolderDocumentViewModel>();
                    folderDocument.SettingAdded += this.On_IEditorDocument_SettingAdded;
                    folderDocument.SettingRemoved += this.On_IEditorDocument_SettingRemoved;

                    await this.SetFolderDocument(folderDocument, folderLocation, collectionLocation);

                    // Open in dock
                    _layoutFactory.AddDocument(folderDocument);
                    _layoutFactory.SetActiveDockable(folderDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, folderDocument);


                    //Close welcome document if configured
                    await this.CloseWelcomeDocument();
                }
                else
                {
                    _layoutFactory.SetActiveDockable(openDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, openDocument);
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);

                throw new Exception($"Opening folder document [{folderLocation}] failed", ex);
                _log.LogInformation($"Document for [{folderLocation}] is already opened");
            }
            finally
            {
                _mainViewViewModel.StatusBarStatus = StatusBarStatus.Idle;
                _mainViewViewModel.StatusBarDetail = string.Empty;
            }
        }

        /// <inheritdoc />
        public async Task OpenFolderDocument(string folderLocation)
        {
            HurlFolderContainer? folderContainer = _editorViewViewModel.Collections.SelectMany(this.GetAllFoldersFromCollection)
                .FirstOrDefault(x => x.AbsoluteLocation == folderLocation);

            if (folderContainer != null)
            {
                await this.OpenFolderDocument(folderLocation,
                    folderContainer.CollectionContainer.Collection.CollectionFileLocation);
            }
        }

        /// <inheritdoc />
        public async Task OpenEnvironmentDocument(string environmentLocation)
        {
            if (_editorViewViewModel.Layout == null)
                throw new ArgumentNullException(nameof(_editorViewViewModel.Layout));

            try
            {
                _log.LogInformation($"Opening environment document for [{environmentLocation}]");
                EnvironmentDocumentViewModel? openDocument =
                    this.GetEnvironmentDocumentByAbsoluteEnvironmentLocation(environmentLocation);

                // Document isn't opened in the dock already
                if (openDocument == null)
                {
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.OpeningEnvironment;
                    _mainViewViewModel.StatusBarDetail = environmentLocation;

                    EnvironmentDocumentViewModel environmentDocument =
                        _documentControlBuilder.Get<EnvironmentDocumentViewModel>();
                    environmentDocument.SettingAdded += this.On_IEditorDocument_SettingAdded;
                    environmentDocument.SettingRemoved += this.On_IEditorDocument_SettingRemoved;

                    await this.SetEnvironmentDocument(environmentDocument, environmentLocation);

                    // Open in dock
                    _layoutFactory.AddDocument(environmentDocument);
                    _layoutFactory.SetActiveDockable(environmentDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, environmentDocument);

                    //Close welcome document if configured
                    await this.CloseWelcomeDocument();
                }
                else
                {
                    _layoutFactory.SetActiveDockable(openDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, openDocument);
                    _log.LogInformation($"Document for [{environmentLocation}] is already opened");
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);

                throw new Exception($"Opening environment document [{environmentLocation}] failed", ex);
            }
            finally
            {
                _mainViewViewModel.StatusBarStatus = StatusBarStatus.Idle;
                _mainViewViewModel.StatusBarDetail = string.Empty;
            }
        }

        /// <inheritdoc />
        public async Task OpenCollectionDocument(string collectionLocation)
        {
            if (_editorViewViewModel.Layout == null)
                throw new ArgumentNullException(nameof(_editorViewViewModel.Layout));

            try
            {
                _log.LogInformation($"Opening collection document for [{collectionLocation}]");
                CollectionDocumentViewModel? openDocument =
                    this.GetCollectionDocumentByAbsoluteCollectionLocation(collectionLocation);

                // Document isn't opened in the dock already
                if (openDocument == null)
                {
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.OpeningCollection;
                    _mainViewViewModel.StatusBarDetail = collectionLocation;

                    CollectionDocumentViewModel collectionDocument =
                        _documentControlBuilder.Get<CollectionDocumentViewModel>();
                    collectionDocument.SettingAdded += this.On_IEditorDocument_SettingAdded;
                    collectionDocument.SettingRemoved += this.On_IEditorDocument_SettingRemoved;

                    await this.SetCollectionDocument(collectionDocument, collectionLocation);

                    // Open in dock
                    _layoutFactory.AddDocument(collectionDocument);
                    _layoutFactory.SetActiveDockable(collectionDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, collectionDocument);

                    //Close welcome document if configured
                    await this.CloseWelcomeDocument();
                }
                else
                {
                    _layoutFactory.SetActiveDockable(openDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, openDocument);
                    _log.LogInformation($"Document for [{collectionLocation}] is already opened");
                }
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);

                throw new Exception($"Opening collection document [{collectionLocation}] failed", ex);
            }
            finally
            {
                _mainViewViewModel.StatusBarStatus = StatusBarStatus.Idle;
                _mainViewViewModel.StatusBarDetail = string.Empty;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SaveFile(FileDocumentViewModel fileDocument)
        {
            if (fileDocument.Document == null) throw new ArgumentNullException(nameof(fileDocument.Document));
            if (fileDocument.FileContainer == null) throw new ArgumentNullException(nameof(fileDocument.FileContainer));

            // Async locked section
            return await _saveLock.LockAsync<bool>(async () =>
            {
                bool result = true;
                _log.LogInformation(
                    $"Attempting to save file [{fileDocument.FileContainer.AbsoluteLocation}] in collection [{fileDocument.FileContainer.CollectionContainer.Collection.CollectionFileLocation}]");

                try
                {
                    _mainViewViewModel.StatusBarDetail = fileDocument.FileContainer.AbsoluteLocation;
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.SavingFile;

                    // Save the .hurl file
                    _log.LogDebug($"Saving .hurl file [{fileDocument.FileContainer.AbsoluteLocation}]");
                    string fileContent = fileDocument.Document.CreateSnapshot().Text;
                    await File.WriteAllTextAsync(fileDocument.FileContainer.AbsoluteLocation, fileContent,
                        Encoding.UTF8);
                    _log.LogDebug($"Saved .hurl file [{fileDocument.FileContainer.AbsoluteLocation}]");

                    // Save the settings
                    _log.LogDebug(
                        $"Saving file settings for [{fileDocument.FileContainer.AbsoluteLocation}] in collection [{fileDocument.FileContainer.CollectionContainer.Collection.CollectionFileLocation}]");
                    HurlCollection hurlCollection =
                        await _collectionService.GetCollectionAsync(fileDocument.FileContainer.CollectionContainer
                            .Collection.CollectionFileLocation);
                    HurlFile? oldCollectionFile = hurlCollection.FileSettings.FirstOrDefault(x =>
                        x.FileLocation == fileDocument.FileContainer.File.FileLocation);

                    // Create new file for saving
                    HurlFile newCollectionFile = new HurlFile(fileDocument.FileContainer.File.FileLocation)
                    {
                        FileTitle = fileDocument.FileContainer.File?.FileTitle ?? string.Empty
                    };

                    List<HurlSettingContainer> settingContainers = fileDocument.SettingSections
                        .Where(x => x.SectionType == HurlSettingSectionType.File)
                        .SelectMany(x => x.SettingContainers).ToList();
                    foreach (HurlSettingContainer settingContainer in settingContainers)
                    {
                        newCollectionFile.FileSettings.Add(settingContainer.Setting);
                    }

                    if (oldCollectionFile != null)
                    {
                        // Replace old file
                        _log.LogDebug(
                            $"Replacing file settings for [{fileDocument.FileContainer.AbsoluteLocation}] in collection [{fileDocument.FileContainer.CollectionContainer.Collection.CollectionFileLocation}]");
                        int oldCollectionFileIdx = hurlCollection.FileSettings.IndexOf(oldCollectionFile);
                        hurlCollection.FileSettings[oldCollectionFileIdx] = newCollectionFile;
                    }
                    else
                    {
                        // Add file
                        _log.LogDebug(
                            $"Adding file settings for [{fileDocument.FileContainer.AbsoluteLocation}] to collection [{fileDocument.FileContainer.CollectionContainer.Collection.CollectionFileLocation}]");
                        hurlCollection.FileSettings.Add(newCollectionFile);
                    }

                    // serialize the collection
                    _log.LogDebug($"Storing collection [{hurlCollection.CollectionFileLocation}]");
                    await _collectionService.StoreCollectionAsync(hurlCollection,
                        hurlCollection.CollectionFileLocation);
                    _log.LogDebug($"Stored collection [{hurlCollection.CollectionFileLocation}]");

                    fileDocument.HasChanges = false;
                    _mainViewViewModel.StatusBarDetail = string.Empty;
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.Idle;

                    _log.LogInformation(
                        $"Saved file [{fileDocument.FileContainer.AbsoluteLocation}] in collection [{fileDocument.FileContainer.CollectionContainer.Collection.CollectionFileLocation}]");
                }
                catch (Exception ex)
                {
                    _log.LogException(ex);
                    _notificationService.Notify(ex);
                    result = false;
                }

                return result;
            });
        }

        /// <inheritdoc />
        public async Task<bool> SaveFolder(FolderDocumentViewModel folderDocument)
        {
            if (folderDocument.FolderContainer == null)
                throw new ArgumentNullException(nameof(folderDocument.FolderContainer));

            // Async locked section
            return await _saveLock.LockAsync<bool>(async () =>
            {
                bool result = true;
                _log.LogInformation(
                    $"Attempting to save folder [{folderDocument.FolderContainer.AbsoluteLocation}] in collection [{folderDocument.FolderContainer.CollectionContainer.Collection.CollectionFileLocation}]");


                try
                {
                    if (folderDocument.FolderContainer == null) return false;

                    _mainViewViewModel.StatusBarDetail = folderDocument.FolderContainer.AbsoluteLocation;
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.SavingFolder;

                    // Save the settings
                    _log.LogDebug(
                        $"Saving folder settings for [{folderDocument.FolderContainer.AbsoluteLocation}] in collection [{folderDocument.FolderContainer.CollectionContainer.Collection.CollectionFileLocation}]");

                    HurlCollection tempHurlCollection = await _collectionService.GetCollectionAsync(folderDocument
                        .FolderContainer.CollectionContainer.Collection.CollectionFileLocation);
                    HurlFolder? oldCollectionFolder = tempHurlCollection.FolderSettings.FirstOrDefault(x =>
                        x.FolderLocation == folderDocument.FolderContainer.Folder.FolderLocation);

                    // Create new folder for saving
                    HurlFolder newCollectionFolder =
                        new HurlFolder(folderDocument.FolderContainer.Folder.FolderLocation);

                    List<HurlSettingContainer> settingContainers = folderDocument.SettingSections
                        .Where(x => x.SectionType == HurlSettingSectionType.Folder)
                        .SelectMany(x => x.SettingContainers).ToList();
                    foreach (HurlSettingContainer settingContainer in settingContainers)
                    {
                        newCollectionFolder.FolderSettings.Add(settingContainer.Setting);
                    }

                    if (oldCollectionFolder != null)
                    {
                        // Replace old folder
                        _log.LogDebug(
                            $"Replacing folder settings for [{folderDocument.FolderContainer?.AbsoluteLocation}] in collection [{folderDocument.FolderContainer?.CollectionContainer.Collection.CollectionFileLocation}]");
                        int oldCollectionFileIdx = tempHurlCollection.FolderSettings.IndexOf(oldCollectionFolder);
                        tempHurlCollection.FolderSettings[oldCollectionFileIdx] = newCollectionFolder;
                    }
                    else
                    {
                        // Add file
                        _log.LogDebug(
                            $"Adding folder settings for [{folderDocument.FolderContainer?.AbsoluteLocation}] to collection [{folderDocument.FolderContainer?.CollectionContainer.Collection.CollectionFileLocation}]");
                        tempHurlCollection.FolderSettings.Add(newCollectionFolder);
                    }

                    // serialize the collection
                    _log.LogDebug($"Storing collection [{tempHurlCollection.CollectionFileLocation}]");
                    await _collectionService.StoreCollectionAsync(tempHurlCollection,
                        tempHurlCollection.CollectionFileLocation);
                    _log.LogDebug($"Stored collection [{tempHurlCollection.CollectionFileLocation}]");

                    // Replace the folder settings in live collection
                    //if (folderDocument.Folder != null)
                    //{
                    //    HurlFolderContainer? liveFolder = this.GetAllFoldersFromCollection(folderDocument.Folder.CollectionContainer)
                    //                                          .FirstOrDefault(x => x.Folder.Location == newCollectionFolder.Location);
                    //    if (liveFolder != null)
                    //    {
                    //        liveFolder.Folder.FolderSettings = newCollectionFolder.FolderSettings;
                    //    }
                    //}

                    // Refresh all opened file documents inherited settings
                    // -> Build a container of the temporary collection
                    // -> Extract the corresponding opened files
                    // -> Rebuild the documents' settings with the settings of the new file
                    HurlCollectionContainer tempCollectionContainer =
                        await _collectionService.GetCollectionContainerAsync(tempHurlCollection);
                    foreach (IDockable dockable in _editorViewViewModel.Documents)
                    {
                        if (dockable is not FileDocumentViewModel openedFileDocument) continue;
                        if (openedFileDocument.FileContainer == null) continue;

                        HurlFileContainer? tempFileContainer = this.GetAllFilesFromCollection(tempCollectionContainer)
                            .FirstOrDefault(
                                x => x.AbsoluteLocation == openedFileDocument.FileContainer.AbsoluteLocation);
                        if (tempFileContainer != null)
                        {
                            await this.LoadSettingsForFile(tempFileContainer, openedFileDocument, false, true);
                        }
                    }

                    folderDocument.HasChanges = false;

                    await this.SetFolderDocument(folderDocument, folderDocument.FolderContainer.AbsoluteLocation,
                        folderDocument.FolderContainer.CollectionContainer.Collection.CollectionFileLocation);

                    _mainViewViewModel.StatusBarDetail = string.Empty;
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.Idle;

                    _log.LogInformation(
                        $"Saved file [{folderDocument.FolderContainer?.AbsoluteLocation}] in collection [{folderDocument.FolderContainer?.CollectionContainer.Collection.CollectionFileLocation}]");
                }
                catch (Exception ex)
                {
                    _log.LogException(ex);
                    _notificationService.Notify(ex);
                    result = false;
                }

                return result;
            });
        }

        /// <inheritdoc />
        public async Task<bool> SaveCollection(CollectionDocumentViewModel collectionDocument)
        {
            if (collectionDocument.CollectionContainer == null)
                throw new ArgumentNullException(nameof(collectionDocument.CollectionContainer));

            // Async locked section
            return await _saveLock.LockAsync<bool>(async () =>
            {
                bool result = true;
                _log.LogInformation(
                    $"Attempting to save collection [{collectionDocument.CollectionContainer.Collection.CollectionFileLocation}]");

                try
                {
                    if (collectionDocument.CollectionContainer == null) return false;

                    _mainViewViewModel.StatusBarDetail =
                        collectionDocument.CollectionContainer.Collection.CollectionFileLocation;
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.SavingCollection;

                    // Save the settings
                    _log.LogDebug(
                        $"Saving collection settings for [{collectionDocument.CollectionContainer.Collection.CollectionFileLocation}]");

                    HurlCollection tempHurlCollection =
                        await _collectionService.GetCollectionAsync(collectionDocument.CollectionContainer.Collection
                            .CollectionFileLocation);
                    List<HurlSettingContainer> settingContainers = collectionDocument.SettingSections
                        .Where(x => x.SectionType == HurlSettingSectionType.Collection)
                        .SelectMany(x => x.SettingContainers).ToList();

                    // Create new folder for saving
                    tempHurlCollection.AdditionalLocations =
                        collectionDocument.CollectionContainer.Collection.AdditionalLocations;
                    tempHurlCollection.ExcludeRootDirectory =
                        collectionDocument.CollectionContainer.Collection.ExcludeRootDirectory;
                    tempHurlCollection.Name = collectionDocument.CollectionContainer.Collection.Name;
                    tempHurlCollection.CollectionSettings = settingContainers.Select(x => (IHurlSetting)x.Setting)
                        .ToObservableCollection();

                    // serialize the collection
                    _log.LogDebug($"Storing collection [{tempHurlCollection.CollectionFileLocation}]");
                    await _collectionService.StoreCollectionAsync(tempHurlCollection,
                        tempHurlCollection.CollectionFileLocation);
                    _log.LogDebug($"Stored collection [{tempHurlCollection.CollectionFileLocation}]");

                    // Refresh all opened file documents inherited settings
                    // -> Build a container of the temporary collection
                    // -> Extract the corresponding opened files
                    // -> Rebuild the documents' settings with the settings of the new file
                    HurlCollectionContainer tempCollectionContainer =
                        await _collectionService.GetCollectionContainerAsync(tempHurlCollection);
                    foreach (IDockable dockable in _editorViewViewModel.Documents)
                    {
                        if (dockable is not FileDocumentViewModel openedFileDocument) continue;
                        if (openedFileDocument.FileContainer == null) continue;

                        HurlFileContainer? tempFileContainer = this.GetAllFilesFromCollection(tempCollectionContainer)
                            .FirstOrDefault(
                                x => x.AbsoluteLocation == openedFileDocument.FileContainer.AbsoluteLocation);
                        if (tempFileContainer != null)
                        {
                            await this.LoadSettingsForFile(tempFileContainer, openedFileDocument, false, true);
                        }
                    }

                    await this.SetCollectionDocument(collectionDocument,
                        collectionDocument.CollectionContainer.Collection.CollectionFileLocation);
                    await this.RefreshCollectionExplorerCollection(collectionDocument.CollectionContainer.Collection
                        .CollectionFileLocation);

                    collectionDocument.HasChanges = false;

                    _mainViewViewModel.StatusBarDetail = string.Empty;
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.Idle;

                    _log.LogInformation(
                        $"Saved file [{collectionDocument.CollectionContainer.Collection.CollectionFileLocation}]");
                }
                catch (Exception ex)
                {
                    _log.LogException(ex);
                    _notificationService.Notify(ex);
                    result = false;
                }

                return result;
            });
        }

        /// <inheritdoc />
        public async Task<bool> SaveEnvironment(EnvironmentDocumentViewModel environmentDocument)
        {
            if (environmentDocument.EnvironmentContainer == null)
                throw new ArgumentNullException(nameof(environmentDocument.EnvironmentContainer));

            // Async locked section
            return await _saveLock.LockAsync<bool>(async () =>
            {
                bool result = true;
                string? activeEnvironmentLocation = _editorViewViewModel.ActiveEnvironment?.EnvironmentFileLocation;
                _log.LogInformation(
                    $"Attempting to save environment [{environmentDocument.EnvironmentContainer.EnvironmentFileLocation}]");


                try
                {
                    if (environmentDocument.EnvironmentContainer == null) return false;

                    _mainViewViewModel.StatusBarDetail =
                        environmentDocument.EnvironmentContainer.EnvironmentFileLocation;
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.SavingEnvironment;

                    // Save the settings
                    _log.LogDebug(
                        $"Saving environment settings for [{environmentDocument.EnvironmentContainer.EnvironmentFileLocation}]");

                    HurlEnvironment tempHurlEnvironment =
                        await _environmentService.GetEnvironmentAsync(environmentDocument.EnvironmentContainer
                            .EnvironmentFileLocation);
                    List<HurlSettingContainer> settingContainers = environmentDocument.SettingSections
                        .Where(x => x.SectionType == HurlSettingSectionType.Environment)
                        .SelectMany(x => x.SettingContainers).ToList();

                    // Create new folder for saving
                    tempHurlEnvironment.Name = environmentDocument.EnvironmentContainer.Environment.Name;
                    tempHurlEnvironment.EnvironmentSettings = settingContainers.Select(x => (IHurlSetting)x.Setting)
                        .ToObservableCollection();

                    // serialize the collection
                    _log.LogDebug($"Storing environment [{tempHurlEnvironment.EnvironmentFileLocation}]");
                    await _environmentService.StoreEnvironmentAsync(tempHurlEnvironment,
                        tempHurlEnvironment.EnvironmentFileLocation);
                    _log.LogDebug($"Stored environment [{tempHurlEnvironment.EnvironmentFileLocation}]");

                    await this.SetEnvironmentDocument(environmentDocument,
                        environmentDocument.EnvironmentContainer.EnvironmentFileLocation);
                    await this.RefreshEnvironmentExplorerEnvironment(environmentDocument.EnvironmentContainer
                        .EnvironmentFileLocation);

                    _mainViewViewModel.StatusBarDetail = string.Empty;
                    _mainViewViewModel.StatusBarStatus = StatusBarStatus.Idle;

                    environmentDocument.HasChanges = false;

                    _log.LogInformation(
                        $"Saved environment [{environmentDocument.EnvironmentContainer.EnvironmentFileLocation}]");
                }
                catch (Exception ex)
                {
                    _log.LogException(ex);
                    _notificationService.Notify(ex);
                    result = false;
                }

                return result;
            });
        }

        /// <inheritdoc />
        public async Task<bool> SaveCurrentDocument()
        {
            if (_editorViewViewModel.DocumentDock?.ActiveDockable == null) return false;

            // Non-IEditor Documents will not return an error
            if (_editorViewViewModel.DocumentDock.ActiveDockable is not IEditorDocument) return true;


            if (_editorViewViewModel.DocumentDock.ActiveDockable is FileDocumentViewModel fileDocument)
            {
                if (!fileDocument.HasChanges) return true;
                return await this.SaveFile(fileDocument);
            }

            if (_editorViewViewModel.DocumentDock.ActiveDockable is FolderDocumentViewModel folderDocument)
            {
                if (!folderDocument.HasChanges) return true;
                return await this.SaveFolder(folderDocument);
            }

            if (_editorViewViewModel.DocumentDock.ActiveDockable is CollectionDocumentViewModel collectionDocument)
            {
                if (!collectionDocument.HasChanges) return true;
                return await this.SaveCollection(collectionDocument);
            }

            if (_editorViewViewModel.DocumentDock.ActiveDockable is EnvironmentDocumentViewModel environmentDocument)
            {
                if (!environmentDocument.HasChanges) return true;
                return await this.SaveEnvironment(environmentDocument);
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<bool> CreateFile(HurlCollectionContainer collectionContainer)
        {
            if (_editorViewViewModel.View?.Window == null)
                return false;

            AddFileWindow addFileWindow = _windowBuilder.Get<AddFileWindow>();
            (string? fileName, HurlFileTemplateContainer? template) =
                await addFileWindow.ShowDialog<(string?, HurlFileTemplateContainer?)>(_editorViewViewModel.View.Window);

            if (fileName == null || template == null) return false;

            if (!fileName.ToNormalized().EndsWith(GlobalConstants.HURL_FILE_EXTENSION.ToNormalized()))
            {
                fileName += GlobalConstants.HURL_FILE_EXTENSION;
            }

            (bool fileCreated, string? absolutePath) =
                await _collectionService.CreateFile(collectionContainer, template, fileName);

            if (!fileCreated || absolutePath == null)
                return false;

            await this.RefreshCollectionExplorerCollection(
                collectionContainer.Collection.CollectionFileLocation);
            await this.OpenFileDocument(absolutePath);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> CreateFile(HurlFolderContainer folderContainer)
        {
            if (_editorViewViewModel.View?.Window == null)
                return false;

            AddFileWindow addFileWindow = _windowBuilder.Get<AddFileWindow>();
            (string? fileName, HurlFileTemplateContainer? template) =
                await addFileWindow.ShowDialog<(string?, HurlFileTemplateContainer?)>(_editorViewViewModel.View.Window);

            if (fileName == null || template == null) return false;

            if (!fileName.ToNormalized().EndsWith(GlobalConstants.HURL_FILE_EXTENSION.ToNormalized()))
            {
                fileName += GlobalConstants.HURL_FILE_EXTENSION;
            }

            (bool fileCreated, string? absolutePath) =
                await _collectionService.CreateFile(folderContainer, template, fileName);

            if (!fileCreated || absolutePath == null)
                return false;

            await this.RefreshCollectionExplorerCollection(
                folderContainer.CollectionContainer.Collection.CollectionFileLocation);
            await this.OpenFileDocument(absolutePath);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> CreateFolder(HurlCollectionContainer rootCollectionContainer)
        {
            if (_editorViewViewModel.View?.Window == null) return false;

            string? folderName = await MessageBox.AskInputDialog(
                _editorViewViewModel.View.Window,
                Localization.Service_EditorService_CreateFolder_MessageBox_Message,
                Localization.Service_EditorService_CreateFolder_MessageBox_Title,
                string.Empty,
                Icon.AddFolder);
            folderName = folderName?.GetValidDirectoryName();
            string? basePath = Path.GetDirectoryName(rootCollectionContainer.Collection.CollectionFileLocation);

            if (string.IsNullOrWhiteSpace(folderName) ||
                basePath == null)
                return false;

            string targetPath = Path.Combine(basePath, folderName);
            if (!await _collectionService.CreateFolder(rootCollectionContainer, targetPath))
                return false;

            await this.RefreshCollectionExplorerCollection(rootCollectionContainer.Collection.CollectionFileLocation);
            return Directory.Exists(targetPath);
        }

        /// <inheritdoc />
        public async Task<bool> CreateFolder(HurlFolderContainer parentFolderContainer)
        {
            if (_editorViewViewModel.View?.Window == null)
                return false;

            string? folderName = await MessageBox.AskInputDialog(
                _editorViewViewModel.View.Window,
                Localization.Service_EditorService_CreateFolder_MessageBox_Message,
                Localization.Service_EditorService_CreateFolder_MessageBox_Title,
                string.Empty,
                Icon.AddFolder);
            folderName = folderName?.GetValidDirectoryName();

            if (string.IsNullOrWhiteSpace(folderName))
                return false;

            string targetPath = Path.Combine(parentFolderContainer.AbsoluteLocation, folderName);
            if (!await _collectionService.CreateFolder(parentFolderContainer.CollectionContainer, targetPath))
                return false;

            await this.RefreshCollectionExplorerCollection(parentFolderContainer.CollectionContainer.Collection
                .CollectionFileLocation);
            return Directory.Exists(targetPath);
        }

        /// <inheritdoc />
        public async Task<bool> CreateCollection()
        {
            Window? window = _editorViewViewModel.View?.Window;
            if (window == null) return false;

            AddCollectionWindow addCollectionWindow = _windowBuilder.Get<AddCollectionWindow>();
            if (addCollectionWindow.ViewModel == null) return false;

            HurlCollection collection = new HurlCollection(string.Empty, string.Empty);
            addCollectionWindow.ViewModel.Collection = collection;

            HurlCollection? resultCollection = await addCollectionWindow.ShowDialog<HurlCollection?>(window);
            if (resultCollection == null) return false;

            bool collectionCreated = await _collectionService.CreateCollection(resultCollection);
            if (!collectionCreated) return false;

            await this.RefreshCollectionExplorerCollections();
            await this.OpenCollectionDocument(resultCollection.CollectionFileLocation);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> CreateEnvironment()
        {
            if (_editorViewViewModel.ActiveEnvironment == null) return false;

            Window? window = _editorViewViewModel.View?.Window;
            if (window == null) return false;

            AddEnvironmentWindow addEnvironmentWindow = _windowBuilder.Get<AddEnvironmentWindow>();
            if (addEnvironmentWindow.ViewModel == null) return false;

            HurlEnvironment environment = new HurlEnvironment(string.Empty);
            addEnvironmentWindow.ViewModel.Environment = environment;

            HurlEnvironment? resultEnvironment = await addEnvironmentWindow.ShowDialog<HurlEnvironment?>(window);

            if (resultEnvironment == null) return false;
            bool environmentCreated = await _environmentService.CreateEnvironment(resultEnvironment);
            if (!environmentCreated) return false;

            await this.RefreshEnvironmentExplorerEnvironments(_editorViewViewModel.ActiveEnvironment
                .EnvironmentFileLocation);
            await this.OpenEnvironmentDocument(resultEnvironment.EnvironmentFileLocation);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteFile(HurlFileContainer fileContainer)
        {
            bool deletedPermanently = false;
            bool movedToTrash = false;

            if (_editorViewViewModel.View?.Window == null)
                return false;

            // Check if open
            FileDocumentViewModel? fileDocument =
                this.GetFileDocumentByAbsoluteFilePath(fileContainer.AbsoluteLocation);
            if (fileDocument != null)
            {
                if (!await _layoutFactory.CloseDockableAsync(fileDocument))
                {
                    return false;
                }
            }

            MessageBoxResult moveToTrashResult = await MessageBox.ShowQuestionYesNoDialog(
                _editorViewViewModel.View.Window,
                fileContainer.AbsoluteLocation,
                Localization.Service_EditorService_DeleteFile_Delete);
            bool moveToTrash = moveToTrashResult == MessageBoxResult.Yes;

            if (!moveToTrash) return true; // true -> User didn't want to delete, so no error

            movedToTrash = await _collectionService.DeleteFile(fileContainer, false);
            if (!movedToTrash)
            {
                MessageBoxResult deletePermanentlyResult = await MessageBox.ShowQuestionYesNoDialog(
                    _editorViewViewModel.View.Window,
                    fileContainer.AbsoluteLocation,
                    Localization.Service_EditorService_DeleteFile_DeletePermanently);
                bool deletePermanently = deletePermanentlyResult == MessageBoxResult.Yes;

                if (deletePermanently)
                {
                    deletedPermanently = await _collectionService.DeleteFile(fileContainer, true);
                }
            }

            // Refresh collection
            await this.RefreshCollectionExplorerCollection(fileContainer.CollectionContainer.Collection
                .CollectionFileLocation);

            return deletedPermanently || movedToTrash;
        }


        /// <inheritdoc />
        public async Task<bool> DeleteFolder(HurlFolderContainer folderContainer)
        {
            if (_editorViewViewModel.View?.Window == null)
                return false;

            // External root folder cannot be deleted
            if (folderContainer.CollectionContainer.Collection.AdditionalLocations.Any(x =>
                    x.Path.ConvertDirectorySeparator() == folderContainer.AbsoluteLocation.ConvertDirectorySeparator()))
            {
                _notificationService.Notify(
                    Model.Notifications.NotificationType.Error,
                    Localization.Service_EditorService_Errors_DeleteFolder_ExternalRootFolderCannotBeDeleted,
                    folderContainer.AbsoluteLocation);
                return false;
            }

            // Check if the folder to be deleted is open
            FolderDocumentViewModel? folderDocument =
                this.GetFolderDocumentByAbsoluteFolderLocation(folderContainer.AbsoluteLocation);
            if (folderDocument != null)
            {
                if (!await _layoutFactory.CloseDockableAsync(folderDocument))
                {
                    return false;
                }
            }

            // Check and close all files
            List<HurlFileContainer> subFiles = this.GetAllFilesFromFolder(folderContainer);
            foreach (HurlFileContainer subFile in subFiles)
            {
                FileDocumentViewModel? fileDocument = this.GetFileDocumentByAbsoluteFilePath(subFile.AbsoluteLocation);
                if (fileDocument != null)
                {
                    if (!await _layoutFactory.CloseDockableAsync(fileDocument))
                    {
                        return true;
                    }
                }
            }

            // Check and close all subfolders
            List<HurlFolderContainer> subFolders = this.GetAllFoldersFromFolder(folderContainer);
            foreach (HurlFolderContainer subFolder in subFolders)
            {
                FolderDocumentViewModel? subFolderDocument =
                    this.GetFolderDocumentByAbsoluteFolderLocation(subFolder.AbsoluteLocation);
                if (subFolderDocument != null)
                {
                    if (!await _layoutFactory.CloseDockableAsync(subFolderDocument))
                    {
                        return true;
                    }
                }
            }

            bool delete = await MessageBox.ShowQuestionYesNoDialog(
                              _editorViewViewModel.View.Window,
                              folderContainer.AbsoluteLocation,
                              Localization
                                  .Service_EditorService_DeleteFolder_Delete) ==
                          MessageBoxResult.Yes;
            if (!delete) return false;

            bool deleted = await _collectionService.DeleteFolder(folderContainer, false);
            if (!deleted)
            {
                bool deletePermanently = await MessageBox.ShowQuestionYesNoDialog(
                                             _editorViewViewModel.View.Window,
                                             folderContainer.AbsoluteLocation,
                                             Localization.Service_EditorService_DeleteFolder_DeletePermanently) ==
                                         MessageBoxResult.Yes;
                if (deletePermanently)
                {
                    deleted = await _collectionService.DeleteFolder(folderContainer, true);
                }
            }

            await this.RefreshCollectionExplorerCollection(folderContainer.CollectionContainer.Collection
                .CollectionFileLocation);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RemoveCollection(HurlCollectionContainer collectionContainer)
        {
            if (_editorViewViewModel.View?.Window == null)
                return false;

            bool remove = await MessageBox.ShowQuestionYesNoDialog(
                              _editorViewViewModel.View.Window,
                              collectionContainer.Collection.CollectionFileLocation,
                              Localization.Service_EditorService_RemoveCollection) ==
                          MessageBoxResult.Yes;
            if (!remove) return false;

            Model.UserSettings.UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);
            if (userSettings.CollectionFiles == null) return false;

            userSettings.CollectionFiles.RemoveAll(x =>
                x.ConvertDirectorySeparator() ==
                collectionContainer.Collection.CollectionFileLocation.ConvertDirectorySeparator());

            await _userSettingsService.StoreUserSettingsAsync();
            await this.RefreshCollectionExplorerCollections();

            return true;
        }


        /// <inheritdoc />
        public async Task<bool> DeleteEnvironment(HurlEnvironmentContainer environmentContainer)
        {
            if (_editorViewViewModel.View?.Window == null) return false;
            if (_editorViewViewModel.ActiveEnvironment == null) return false;

            bool deletedPermanently = false;
            bool movedToTrash = false;

            // Check if open
            EnvironmentDocumentViewModel? environmentDocument =
                this.GetEnvironmentDocumentByAbsoluteEnvironmentLocation(environmentContainer.EnvironmentFileLocation);
            if (environmentDocument != null)
            {
                if (!await _layoutFactory.CloseDockableAsync(environmentDocument))
                {
                    return true;
                }
            }

            // Try to delete via recycle bin
            bool moveToTrashResult = await MessageBox.ShowQuestionYesNoDialog(
                _editorViewViewModel.View.Window,
                environmentContainer.EnvironmentFileLocation,
                Localization.Service_EditorService_DeleteEnvironment_Delete) == MessageBoxResult.Yes;
            if (!moveToTrashResult) return false;

            movedToTrash = await _environmentService.DeleteEnvironment(environmentContainer, false);
            
            if (!movedToTrash)
            {
                bool deletePermanently = await MessageBox.ShowQuestionYesNoDialog(
                    _editorViewViewModel.View.Window,
                    environmentContainer.EnvironmentFileLocation,
                    Localization.Service_EditorService_DeleteEnvironment_DeletePermanently) == MessageBoxResult.Yes;
                if (deletePermanently)
                {
                    deletedPermanently = await _environmentService.DeleteEnvironment(environmentContainer, true);
                }
            }

            await this.RefreshEnvironmentExplorerEnvironments(_editorViewViewModel.ActiveEnvironment
                .EnvironmentFileLocation);
            
            return deletedPermanently || movedToTrash;
        }

        /// <inheritdoc />
        public async Task Start()
        {
            if (_mainViewViewModel == null) throw new ArgumentNullException(nameof(_mainViewViewModel));
            if (_mainViewViewModel.LoadingView == null) throw new ArgumentNullException(nameof(_mainViewViewModel));

            Model.UserSettings.UserSettings? userSettings = await _userSettingsService.GetUserSettingsAsync(false);
            Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(true);

            if (userSettings == null) throw new ArgumentNullException(nameof(UserSettings));

            // Load enviroments
            _mainViewViewModel.LoadingView.CurrentActivity = LoadingViewStep.LoadingEnvironments;
            _editorViewViewModel.Environments = await _environmentService.GetEnvironmentContainersAsync();
            _editorViewViewModel.ActiveEnvironment =
                _editorViewViewModel.Environments.FirstOrDefault(x =>
                    x.EnvironmentFileLocation == uiState?.ActiveEnvironmentFile) ??
                _editorViewViewModel.Environments.FirstOrDefault();

            // Load collections and open files from previous session
            _mainViewViewModel.LoadingView.CurrentActivity = LoadingViewStep.LoadingCollections;
            _editorViewViewModel.Collections = await _collectionService.GetCollectionContainersAsync();

            _mainViewViewModel.LoadingView.CurrentActivity = LoadingViewStep.OpeningPreviousSessionFiles;
            _editorViewViewModel.Layout = _layoutFactory.CreateLayout();
            await this.LoadInitialUserSettings();
            await this.OpenInitialDocuments();

            _mainViewViewModel.InitializationCompleted = true;
            _mainViewViewModel.LoadingView.CurrentActivity = LoadingViewStep.Finished;
        }

        /// <inheritdoc />
        public async Task<bool> Shutdown()
        {
            List<string> openedFiles = new List<string>();
            List<IDockable> openDockables = new List<IDockable>();
            foreach (IDockable document in _editorViewViewModel.Documents)
            {
                if (document is IEditorDocument editorDocument && editorDocument.HurlContainer != null)
                {
                    openDockables.Add(document);
                    openedFiles.Add(editorDocument.HurlContainer.GetPath());
                }
            }

            _uiStateService.SetOpenedDocuments(openedFiles);

            // Close dockables
            foreach (IDockable dockable in openDockables)
            {
                if (!await _layoutFactory.CloseDockableAsync(dockable))
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public async Task RefreshCollectionExplorerCollections()
        {
            _editorViewViewModel.Collections.RemoveAll(x => true);
            _editorViewViewModel.Collections.AddRange(await _collectionService.GetCollectionContainersAsync());
        }

        /// <inheritdoc />
        public async Task RefreshCollectionExplorerCollection(string collectionLocation)
        {
            HurlCollectionContainer? collectionContainer =
                _editorViewViewModel.Collections.FirstOrDefault(x =>
                    x.Collection.CollectionFileLocation == collectionLocation);
            if (collectionContainer == null) return;

            int collectionIndex = _editorViewViewModel.Collections.IndexOf(collectionContainer);
            if (collectionIndex == -1) return;

            HurlCollection collection = await _collectionService.GetCollectionAsync(collectionLocation);
            _editorViewViewModel.Collections[collectionIndex] =
                await _collectionService.GetCollectionContainerAsync(collection);
        }

        /// <inheritdoc />
        public async Task RefreshEnvironmentExplorerEnvironments(string? activeEnvironmentLocation)
        {
            _editorViewViewModel.Environments.RemoveAll(x => true);
            _editorViewViewModel.Environments.AddRange(await _environmentService.GetEnvironmentContainersAsync());

            // Re-set the active environment by location
            _editorViewViewModel.ActiveEnvironment =
                _editorViewViewModel.Environments.FirstOrDefault(x =>
                    x.EnvironmentFileLocation == activeEnvironmentLocation) ??
                _editorViewViewModel.Environments.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task RefreshEnvironmentExplorerEnvironment(string environmentLocation)
        {
            HurlEnvironmentContainer? environmentContainer =
                _editorViewViewModel?.Environments.FirstOrDefault(x => x.EnvironmentFileLocation == environmentLocation);
            if (environmentContainer == null) return;

            int environmentIndex = _editorViewViewModel.Environments.IndexOf(environmentContainer);
            if (environmentIndex == -1) return;

            HurlEnvironment environment = await _environmentService.GetEnvironmentAsync(environmentLocation);
            _editorViewViewModel.Environments[environmentIndex] =
                await _environmentService.GetEnvironmentContainerAsync(environment);

            // Re-set the active environment by location
            _editorViewViewModel.ActiveEnvironment =
                _editorViewViewModel.Environments.FirstOrDefault(x =>
                    x.EnvironmentFileLocation == environmentLocation) ??
                _editorViewViewModel.Environments.FirstOrDefault();
        }


        /// <summary>
        /// Layout factory history -> enqueue dockables on change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_LayoutFactory_ActiveDockableChanged(object? sender,
            Dock.Model.Core.Events.ActiveDockableChangedEventArgs e)
        {
            if (_fileHistoryBlocked) return;
            if (e.Dockable is not DocumentBase document) return;
            _editorViewViewModel.ActiveDocument = document;

            if (!_editorViewViewModel.InitializationCompleted) return;
            _editorViewViewModel.DocumentHistory.Push(_editorViewViewModel.PreviousDocument);

            while (_editorViewViewModel.DocumentFuture.Count > 0)
            {
                _editorViewViewModel.DocumentFuture.Pop();
            }
        }

        /// <summary>
        /// Moves a file to a new location and migrates its settings to the target collection
        /// </summary>
        /// <param name="originalFile"></param>
        /// <param name="originalCollectionPath"></param>
        /// <param name="targetCollectionPath"></param>
        /// <param name="targetAbsoluteFilePath"></param>
        /// <returns></returns>
        private async Task<(bool, bool)> MoveFile(HurlFileContainer originalFile, string originalCollectionPath, string targetCollectionPath, string targetAbsoluteFilePath)
        {
            bool storeCollectionsNeeded = false;
            bool reopenFile = false;
            HurlCollection tempOriginalCollection = await _collectionService.GetCollectionAsync(originalCollectionPath);
            HurlCollection tempTargetCollection = await _collectionService.GetCollectionAsync(targetCollectionPath);

            HurlCollectionContainer tempOriginalCollectionContainer =
                await _collectionService.GetCollectionContainerAsync(tempOriginalCollection);
            HurlCollectionContainer tempTargetCollectionContainer =
                await _collectionService.GetCollectionContainerAsync(tempTargetCollection);

            string originalFilePath = originalFile.AbsoluteLocation;
            string? originalCollectionDirectory = Path.GetDirectoryName(originalCollectionPath);
            string? targetCollectionDirectory = Path.GetDirectoryName(targetCollectionPath);
            string? targetRelativeFilePath;

            if (targetCollectionDirectory != null)
            {
                targetRelativeFilePath = Path.GetRelativePath(targetCollectionDirectory, targetAbsoluteFilePath);

                // Check if relative path is distinct from collection path
                // -> target absolute file path is the path to go with
                if (!PathExtensions.IsChildOfDirectory(targetAbsoluteFilePath, targetCollectionDirectory))
                {
                    targetRelativeFilePath = targetAbsoluteFilePath;
                }
            }
            else return (false, reopenFile);

            // Special case: file moved to its own parent -> mark success and return
            if (targetAbsoluteFilePath == originalFilePath) return (true, false);

            if (File.Exists(targetAbsoluteFilePath))
            {
                _notificationService.Notify(
                    Model.Notifications.NotificationType.Error,
                    Localization.Service_EditorService_Errors_MoveFile_AlreadyExists,
                    targetAbsoluteFilePath);
                return (false, reopenFile);
            }

            // Close file if necessary
            FileDocumentViewModel? openedFileDocument =
                this.GetFileDocumentByAbsoluteFilePath(originalFile.AbsoluteLocation);
            if (openedFileDocument != null)
            {
                if (!await _layoutFactory.CloseDockableAsync(openedFileDocument))
                {
                    return (false, reopenFile);
                }
                else
                {
                    reopenFile = true;
                }
            }

            // Create a document for taking over the ui state settings
            FileDocumentViewModel originalFileDocument = _documentControlBuilder.Get<FileDocumentViewModel>();
            await this.SetFileDocument(originalFileDocument, originalFilePath, originalCollectionPath);

            // Remove file settings from old collection
            HurlFile? fileInOriginalCollection = tempOriginalCollection.FileSettings.FirstOrDefault(x =>
                x.FileLocation.ConvertDirectorySeparator() ==
                originalFile.File.FileLocation.ConvertDirectorySeparator());
            if (fileInOriginalCollection != null)
            {
                tempOriginalCollection.FileSettings.Remove(fileInOriginalCollection);

                // Check, if the settings are also present in the new collection and remove them, if necessary
                // ( needed for moving within the same collection )
                HurlFile? originalFileInTargetCollection = tempTargetCollection.FileSettings.FirstOrDefault(x =>
                    x.FileLocation.ConvertDirectorySeparator() ==
                    originalFile.File.FileLocation.ConvertDirectorySeparator());
                if (originalFileInTargetCollection != null)
                {
                    tempTargetCollection.FileSettings.Remove(originalFileInTargetCollection);
                }

                // Add file settings to new collection
                fileInOriginalCollection.FileLocation = targetRelativeFilePath;
                tempTargetCollection.FileSettings.Add(fileInOriginalCollection);

                storeCollectionsNeeded = true;
            }

            // Move file
            await Task.Run(() => File.Move(originalFilePath, targetAbsoluteFilePath));

            // Store collections if needed
            if (storeCollectionsNeeded)
            {
                await _collectionService.StoreCollectionAsync(tempOriginalCollection, originalCollectionPath);
                await _collectionService.StoreCollectionAsync(tempTargetCollection, targetCollectionPath);
            }


            // Set ui state of new file
            FileDocumentViewModel targetFileDocument = _documentControlBuilder.Get<FileDocumentViewModel>();
            await this.SetFileDocument(targetFileDocument, targetAbsoluteFilePath, targetCollectionPath);

            List<HurlSettingContainer> originalSettingContainers = originalFileDocument.SettingSections
                .Where(x => x.SectionType == HurlSettingSectionType.File)
                .SelectMany(x => x.SettingContainers)
                .ToList();
            List<HurlSettingContainer> targetSettingContainers = targetFileDocument.SettingSections
                .Where(x => x.SectionType == HurlSettingSectionType.File)
                .SelectMany(x => x.SettingContainers)
                .ToList();

            // Transfer section collapse states
            string? sectionId = targetFileDocument.SettingSections
                .FirstOrDefault(x => x.SectionType == HurlSettingSectionType.File)?.GetId();
            string? originalSectionId = originalFileDocument.SettingSections
                .FirstOrDefault(x => x.SectionType == HurlSettingSectionType.File)?.GetId();
            bool? sectionCollapsed = originalFileDocument.SettingSections
                .FirstOrDefault(x => x.SectionType == HurlSettingSectionType.File)?.Collapsed;
            if (sectionId != null && originalSectionId != null && sectionCollapsed.HasValue)
            {
                _uiStateService.SetSettingSectionCollapsedState(sectionId, sectionCollapsed.Value);
                _uiStateService.RemoveSettingSectionCollapsedState(originalSectionId);
            }

            // For each setting in this file context -
            // -> add the collapse state of the originals' setting 
            // -> remove the collapse state of the originals' setting
            for (int i = 0; i < originalSettingContainers.Count; i++)
            {
                HurlSettingContainer? originalSettingContainer = originalSettingContainers.Get(i);
                string? newId = targetSettingContainers.Get(i)?.GetId();

                if (originalSettingContainer != null && newId != null)
                {
                    _uiStateService.SetSettingCollapsedState(newId, originalSettingContainer.Collapsed);
                    _uiStateService.RemoveSettingCollapsedState(originalSettingContainer.GetId());
                }
            }

            return (true, reopenFile);
        }

        /// <summary>
        /// Moves a folder to a new location and migrates its settings and all children to the target collection
        /// </summary>
        /// <param name="originalFolder"></param>
        /// <param name="originalCollectionPath"></param>
        /// <param name="targetCollectionPath"></param>
        /// <param name="targetAbsoluteFolderPath"></param>
        /// <returns></returns>
        private async Task<(bool, List<string>)> MoveFolder(HurlFolderContainer originalFolder,
            string originalCollectionPath, string targetCollectionPath, string targetAbsoluteFolderPath)
        {
            bool storeCollectionsNeeded = false;
            bool moveSuccessful = true;
            List<string> reopenPaths = new List<string>();

            _log.LogDebug(
                $"Moving [{originalFolder}] of collection [{originalCollectionPath}] to [{targetAbsoluteFolderPath}] in collection [{targetCollectionPath}]");
            HurlCollection tempOriginalCollection = await _collectionService.GetCollectionAsync(originalCollectionPath);
            HurlCollection tempTargetCollection = await _collectionService.GetCollectionAsync(targetCollectionPath);

            HurlCollectionContainer tempOriginalCollectionContainer =
                await _collectionService.GetCollectionContainerAsync(tempOriginalCollection);
            HurlCollectionContainer tempTargetCollectionContainer =
                await _collectionService.GetCollectionContainerAsync(tempTargetCollection);

            string originalFolderPath = originalFolder.AbsoluteLocation;
            string? originalCollectionDirectory = Path.GetDirectoryName(originalCollectionPath);
            string? targetCollectionDirectory = Path.GetDirectoryName(targetCollectionPath);
            string? targetRelativeFolderPath;

            // Special case: folder moved to its own parent -> mark success and return
            if (originalFolderPath == targetAbsoluteFolderPath) return (true, reopenPaths);

            // "External" root folders cannot be moved
            if (tempOriginalCollection.AdditionalLocations.Any(x =>
                    x.Path.ConvertDirectorySeparator() == originalFolder.AbsoluteLocation.ConvertDirectorySeparator()))
            {
                _notificationService.Notify(
                    Model.Notifications.NotificationType.Error,
                    Localization.Service_EditorService_Errors_MoveFolder_ExternalRootFolderCannotBeMoved,
                    targetAbsoluteFolderPath);
                return (false, reopenPaths);
            }

            if (targetCollectionDirectory != null)
            {
                targetRelativeFolderPath = Path.GetRelativePath(targetCollectionDirectory, targetAbsoluteFolderPath);

                // Check if relative path is distinct from collection path
                // -> target absolute folder path is the path to go with
                if (!PathExtensions.IsChildOfDirectory(targetAbsoluteFolderPath, targetCollectionDirectory))
                {
                    targetRelativeFolderPath = targetAbsoluteFolderPath;
                }
            }
            else return (false, reopenPaths);

            // If target directory already exists, return
            if (Directory.Exists(targetAbsoluteFolderPath))
            {
                _notificationService.Notify(
                    Model.Notifications.NotificationType.Error,
                    Localization.Service_EditorService_Errors_MoveFolder_AlreadyExists,
                    targetAbsoluteFolderPath);
                return (false, reopenPaths);
            }

            // Close folder if necessary
            FolderDocumentViewModel? openedFolderDocument =
                this.GetFolderDocumentByAbsoluteFolderLocation(originalFolder.AbsoluteLocation);
            if (openedFolderDocument != null)
            {
                if (!await _layoutFactory.CloseDockableAsync(openedFolderDocument))
                {
                    return (false, reopenPaths);
                }
                else
                {
                    reopenPaths.Add(targetAbsoluteFolderPath);
                }
            }

            // Remove folder settings from old collection
            HurlFolder? folderInOriginalCollection = tempOriginalCollection.FolderSettings.FirstOrDefault(
                x => x.FolderLocation.ConvertDirectorySeparator() ==
                     originalFolder.Folder.FolderLocation.ConvertDirectorySeparator());

            if (folderInOriginalCollection != null)
            {
                tempOriginalCollection.FolderSettings.Remove(folderInOriginalCollection);

                // Check, if the settings are also present in the new collection and remove them, if necessary
                // ( needed for moving within the same collection )
                HurlFolder? originalFolderInTargetCollection = tempTargetCollection.FolderSettings.FirstOrDefault(
                    x => x.FolderLocation.ConvertDirectorySeparator() ==
                         originalFolder.Folder.FolderLocation.ConvertDirectorySeparator());

                if (originalFolderInTargetCollection != null)
                {
                    tempTargetCollection.FolderSettings.Remove(originalFolderInTargetCollection);
                }

                // Add folder settings to new collection
                folderInOriginalCollection.FolderLocation = targetRelativeFolderPath;
                tempTargetCollection.FolderSettings.Add(folderInOriginalCollection);

                storeCollectionsNeeded = true;
            }

            // Save the sub files' setting containers for later rebuild of the ui state
            Dictionary<string, HurlSettingSection?> originalFileFolderSettingSections =
                new Dictionary<string, HurlSettingSection?>();
            List<HurlFileContainer> allOriginalFolderSubFiles = this.GetAllFilesFromFolder(originalFolder);
            foreach (HurlFileContainer originalSubFile in allOriginalFolderSubFiles)
            {
                // Prepare document of original file for later transfer of ui state
                FileDocumentViewModel originalSubFileDocumentViewModel =
                    _documentControlBuilder.Get<FileDocumentViewModel>();
                await this.SetFileDocument(originalSubFileDocumentViewModel, originalSubFile.AbsoluteLocation,
                    originalCollectionPath);

                HurlSettingSection? originalFolderSettingSection = originalSubFileDocumentViewModel.SettingSections
                    .FirstOrDefault(x => x.CollectionComponent is HurlFolderContainer folder &&
                                         folder.AbsoluteLocation == originalSubFile.FolderContainer.AbsoluteLocation);

                originalFileFolderSettingSections.Add(originalSubFile.AbsoluteLocation, originalFolderSettingSection);
            }


            // Create target folder
            Directory.CreateDirectory(targetAbsoluteFolderPath);

            // Store collections if needed
            if (storeCollectionsNeeded)
            {
                await _collectionService.StoreCollectionAsync(tempOriginalCollection, originalCollectionPath);
                await _collectionService.StoreCollectionAsync(tempTargetCollection, targetCollectionPath);
            }


            // Move Sub-files to new folder
            foreach (HurlFileContainer subFile in originalFolder.Files)
            {
                string subFileAbsoluteTargetPath =
                    Path.Combine(targetAbsoluteFolderPath, Path.GetFileName(subFile.AbsoluteLocation))
                        .ConvertDirectorySeparator();

                (bool subFileMoveSuccessful, bool subFileReopenNeeded) =
                    await this.MoveFile(subFile, originalCollectionPath, targetCollectionPath,
                        subFileAbsoluteTargetPath);
                if (!subFileMoveSuccessful)
                {
                    moveSuccessful = false;
                }

                if (subFileReopenNeeded)
                {
                    reopenPaths.Add(subFileAbsoluteTargetPath);
                }
            }

            // Also move non-tracked files
            string[] nonTrackedFiles = Directory.GetFiles(originalFolderPath);
            foreach (string nonTrackedFile in nonTrackedFiles)
            {
                string absoluteNonTrackedFileTargetPath = Path
                    .Combine(targetAbsoluteFolderPath, Path.GetFileName(nonTrackedFile)).ConvertDirectorySeparator();
                _log.LogInformation(
                    $"Moving untracked file [{nonTrackedFile}] to  [{absoluteNonTrackedFileTargetPath}]");
                await Task.Run(() => File.Move(nonTrackedFile, absoluteNonTrackedFileTargetPath));
            }

            // Move Sub-folders to new folder
            foreach (HurlFolderContainer subFolder in originalFolder.Folders)
            {
                string subFolderName = new DirectoryInfo(subFolder.AbsoluteLocation).Name;
                string absoluteFolderTargetPath =
                    Path.Combine(targetAbsoluteFolderPath, subFolderName).ConvertDirectorySeparator();

                (bool subFolderMoveSuccessful, List<string> subFolderReopenPaths) =
                    await this.MoveFolder(subFolder, originalCollectionPath, targetCollectionPath,
                        absoluteFolderTargetPath);
                if (!subFolderMoveSuccessful)
                {
                    moveSuccessful = false;
                }

                reopenPaths.AddRange(subFolderReopenPaths);
            }

            // Rebuild ui state in all files (including subfolders)
            // -> Transfer the collapse and enabled states of all original files within this folder (and in its context)
            foreach (HurlFileContainer originalSubFile in allOriginalFolderSubFiles)
            {
                string subFileAbsoluteTargetPath = Path.Combine(targetAbsoluteFolderPath,
                        Path.GetRelativePath(originalFolderPath, originalSubFile.AbsoluteLocation))
                    .ConvertDirectorySeparator();

                FileDocumentViewModel targetSubFileDocumentViewModel =
                    _documentControlBuilder.Get<FileDocumentViewModel>();
                await this.SetFileDocument(targetSubFileDocumentViewModel, subFileAbsoluteTargetPath,
                    targetCollectionPath);

                HurlSettingSection? targetFolderSettingSection = targetSubFileDocumentViewModel.SettingSections
                    .FirstOrDefault(x => x.CollectionComponent is HurlFolderContainer folder &&
                                         folder.AbsoluteLocation == targetAbsoluteFolderPath);

                if (targetFolderSettingSection != null &&
                    originalFileFolderSettingSections.TryGetValue(originalSubFile.AbsoluteLocation,
                        out HurlSettingSection? originalFolderSettingSection) &&
                    originalFolderSettingSection != null)
                {
                    // Transfer section collapse state
                    _uiStateService.SetSettingSectionCollapsedState(targetFolderSettingSection.GetId(),
                        originalFolderSettingSection.Collapsed);
                    _uiStateService.RemoveSettingSectionCollapsedState(originalFolderSettingSection.GetId());

                    // Transfer each settings' collapse-/ and enabled state
                    for (int i = 0; i < originalFolderSettingSection.SettingContainers.Count; i++)
                    {
                        HurlSettingContainer? originalSettingContainer =
                            originalFolderSettingSection.SettingContainers.Get(i);
                        string? newId = targetFolderSettingSection.SettingContainers.Get(i)?.GetId();
                        string? originalId = originalFolderSettingSection.SettingContainers.Get(i)?.GetId();

                        if (newId != null && originalSettingContainer != null)
                        {
                            _uiStateService.SetSettingCollapsedState(newId, originalSettingContainer.Collapsed);
                            _uiStateService.SetSettingEnabledState(newId, originalSettingContainer.IsEnabled);
                        }

                        if (originalId != null && originalSettingContainer != null)
                        {
                            _uiStateService.RemoveSettingCollapsedState(originalId);
                            _uiStateService.RemoveSettingEnabledState(originalId);
                        }
                    }
                }
            }


            // Delete old folder
            Directory.Delete(originalFolder.AbsoluteLocation);

            return (moveSuccessful, reopenPaths);
        }

        /// <summary>
        /// Fills a file document view model with settings and file content
        /// </summary>
        /// <param name="fileDocument"></param>
        /// <param name="fileLocation"></param>
        /// <param name="collectionLocation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task SetFileDocument(FileDocumentViewModel fileDocument, string fileLocation,
            string collectionLocation)
        {
            HurlCollection collection = await _collectionService.GetCollectionAsync(collectionLocation);
            HurlCollectionContainer collectionContainer =
                await _collectionService.GetCollectionContainerAsync(collection);
            HurlFileContainer? file = this.GetAllFilesFromCollection(collectionContainer)
                .Where(x => x.AbsoluteLocation == fileLocation).FirstOrDefault();
            if (file == null) throw new ArgumentNullException(nameof(file));

            fileDocument.Id = file.GetId();
            fileDocument.FileContainer = file;
            fileDocument.Document =
                new AvaloniaEdit.Document.TextDocument(
                    await File.ReadAllTextAsync(file.AbsoluteLocation, Encoding.UTF8));

            // Load settings
            await this.LoadSettingsForFile(file, fileDocument, true, true);
        }

        /// <summary>
        /// Loads settings for a file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileDocument"></param>
        /// <param name="reloadFileSettings">load the file specific settings</param>
        /// <param name="reloadInheritedSettings">load settings inherited from environment, collection and folders</param>
        /// <returns></returns>
        private async Task LoadSettingsForFile(HurlFileContainer? file, FileDocumentViewModel fileDocument,
            bool reloadFileSettings, bool reloadInheritedSettings)
        {
            if (file == null) return;

            await Task.Run(() =>
            {
                if (reloadInheritedSettings)
                {
                    this.LoadInheritedSettingsForFile(file, fileDocument);
                }

                if (reloadFileSettings)
                {
                    this.LoadFileSettingsForFile(file, fileDocument);
                }
            });

            await this.EvaluateSettingInheritancesAsync(fileDocument);
        }

        /// <summary>
        /// Reevaluate settings when one is removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_IEditorDocument_SettingRemoved(object? sender,
            Model.EventArgs.SettingEvaluationChangedEventArgs e)
        {
            if (sender != null && sender is IEditorDocument document)
            {
                // Unbind events for reevaluation
                e.Setting.SettingEnabledChanged -= this.On_HurlSettingContainer_SettingEnabledChanged;
                e.Setting.SettingOrderChanged -= this.On_HurlSettingContainer_SettingOrderChanged;
                e.Setting.SettingKeyChanged -= this.On_HurlSettingContainer_SettingKeyChanged;
                e.Setting.SettingCollapsedChanged -= this.On_HurlSettingContainer_SettingCollapsedChanged;
                e.Setting.SettingChanged -= this.On_HurlSettingContainer_SettingChanged;

                await this.EvaluateSettingInheritancesAsync(document);
            }
        }


        /// <summary>
        /// On Active environment change 
        /// -> rebuild inherited settings for all open documents and reevaluate their inheritances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_EditorViewViewModel_ActiveEnvironmentChanged(object? sender,
            Model.EventArgs.ActiveEnvironmentChangedEventArgs e)
        {
            //    if (!_mainViewViewModel.InitializationCompleted) return;
            //    if (!_editorViewViewModel.InitializationCompleted) return;

            // async lock
            await _saveLock.LockAsync(async () =>
            {
                try
                {
                    foreach (HurlEnvironmentContainer environmentContainer in _editorViewViewModel.Environments)
                    {
                        environmentContainer.IsActiveEnvironment = false;
                    }

                    if (e.Environment != null)
                    {
                        _uiStateService.SetActiveEnvironment(e.Environment.EnvironmentFileLocation);
                        e.Environment.IsActiveEnvironment = true;
                    }

                    foreach (IDockable dockable in _editorViewViewModel.Documents)
                    {
                        if (dockable is not FileDocumentViewModel fileDocument) continue;
                        if (fileDocument.FileContainer == null) continue;

                        // Reload inherited settings
                        await this.LoadSettingsForFile(fileDocument.FileContainer, fileDocument, false, true);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogException(ex);
                    _notificationService.Notify(ex);
                }
            });
        }

        /// <summary>
        /// Reevaluate settings when a new one is added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_IEditorDocument_SettingAdded(object? sender,
            Model.EventArgs.SettingEvaluationChangedEventArgs e)
        {
            if (sender != null && sender is IEditorDocument document)
            {
                // Bind events for reevaluation
                e.Setting.SettingEnabledChanged += this.On_HurlSettingContainer_SettingEnabledChanged;
                e.Setting.SettingOrderChanged += this.On_HurlSettingContainer_SettingOrderChanged;
                e.Setting.SettingKeyChanged += this.On_HurlSettingContainer_SettingKeyChanged;
                e.Setting.SettingCollapsedChanged += this.On_HurlSettingContainer_SettingCollapsedChanged;
                e.Setting.SettingChanged += this.On_HurlSettingContainer_SettingChanged;

                await this.EvaluateSettingInheritancesAsync(document);
            }
        }

        /// <summary>
        /// Consolidates a list of settings for a given fileDocument into the document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileDocument"></param>
        private void LoadFileSettingsForFile(HurlFileContainer file, FileDocumentViewModel fileDocument)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);

            fileDocument.SettingSections.RemoveAll(x => x.SectionType == HurlSettingSectionType.File);

            // File settings
            HurlSettingSection fileSection = new HurlSettingSection(fileDocument, HurlSettingSectionType.File, file);
            fileSection.SettingContainers.AddRangeIfNotNull(
                file.File?
                    .FileSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x =>
                        new HurlSettingContainer(fileDocument, fileSection, x, false, true, EnableType.Setting)));
            fileDocument.SettingSections.Insert(0, fileSection);


            // Apply sections' ui state and bind relevant events
            string sectionId = fileSection.GetId();
            if (uiState != null && uiState.SettingSectionCollapsedStates.ContainsKey(sectionId))
            {
                fileSection.Collapsed = uiState.SettingSectionCollapsedStates[sectionId];
            }

            fileSection.SettingSectionCollapsedChanged -= this.On_HurlSettingSection_SettingSectionCollapsedChanged;
            fileSection.SettingSectionCollapsedChanged += this.On_HurlSettingSection_SettingSectionCollapsedChanged;

            // Inheritance behavior
            this.BindSettingContainers(fileSection.SettingContainers.ToList(), true);
        }

        /// <summary>
        /// Consolidates a list of settings for a given fileDocument into the document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileDocument"></param>
        private void LoadInheritedSettingsForFile(HurlFileContainer file, FileDocumentViewModel fileDocument)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);
            List<HurlSettingSection> sections = new List<HurlSettingSection>();
            int insertIdx = fileDocument.SettingSections.Count(x => x.SectionType == HurlSettingSectionType.File);

            fileDocument.SettingSections.RemoveAll(x => x.SectionType != HurlSettingSectionType.File);

            // Environment settings
            HurlSettingSection environmentSection = new HurlSettingSection(fileDocument,
                HurlSettingSectionType.Environment, _editorViewViewModel.ActiveEnvironment);
            environmentSection.SettingContainers.AddRangeIfNotNull(
                _editorViewViewModel
                    .ActiveEnvironment?
                    .Environment
                    .EnvironmentSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x =>
                        new HurlSettingContainer(fileDocument, environmentSection, x, true, false,
                            EnableType.Container)));
            fileDocument.SettingSections.Insert(insertIdx, environmentSection);
            sections.Insert(0, environmentSection);

            // Collection settings
            HurlSettingSection collectionSection = new HurlSettingSection(fileDocument,
                HurlSettingSectionType.Collection, file.CollectionContainer);
            collectionSection.SettingContainers.AddRangeIfNotNull(
                file.CollectionContainer
                    .Collection
                    .CollectionSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x =>
                        new HurlSettingContainer(fileDocument, collectionSection, x, true, false,
                            EnableType.Container)));
            fileDocument.SettingSections.Insert(insertIdx, collectionSection);
            sections.Insert(0, collectionSection);

            // Folder settings
            List<HurlFolderContainer> collectionFolders = this.GetParentFolders(file.FolderContainer);
            foreach (HurlFolderContainer folder in collectionFolders)
            {
                if (folder.AbsoluteLocation.ToLower().ConvertDirectorySeparator() != Path
                        .GetDirectoryName(file.CollectionContainer.Collection.CollectionFileLocation)?.ToLower()
                        .ConvertDirectorySeparator())
                {
                    HurlSettingSection folderSection =
                        new HurlSettingSection(fileDocument, HurlSettingSectionType.Folder, folder);
                    folderSection.SettingContainers.AddRangeIfNotNull(
                        folder.Folder?
                            .FolderSettings
                            .Where(x => x is BaseSetting)
                            .Select(x => (BaseSetting)x)
                            .Select(x =>
                                new HurlSettingContainer(fileDocument, folderSection, x, true, false,
                                    EnableType.Container)));

                    fileDocument.SettingSections.Insert(insertIdx, folderSection);
                    sections.Insert(0, folderSection);
                }
            }

            // Apply sections' ui state and bind relevant events
            foreach (HurlSettingSection section in sections)
            {
                string sectionId = section.GetId();
                if (uiState != null && uiState.SettingSectionCollapsedStates.ContainsKey(sectionId))
                {
                    section.Collapsed = uiState.SettingSectionCollapsedStates[sectionId];
                }

                section.SettingSectionCollapsedChanged -= this.On_HurlSettingSection_SettingSectionCollapsedChanged;
                section.SettingSectionCollapsedChanged += this.On_HurlSettingSection_SettingSectionCollapsedChanged;
            }


            List<HurlSettingContainer> allSettingContainers = sections.SelectMany(x => x.SettingContainers).ToList();

            // Inheritance behavior
            this.BindSettingContainers(allSettingContainers, true);
        }

        /// <summary>
        /// Consolidates a list of settings for a given folderDocument into the document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileDocument"></param>
        private async Task LoadSettingsForCollection(HurlCollectionContainer collection,
            CollectionDocumentViewModel collectionDocument)
        {
            if (collection == null) return;

            await Task.Run(() =>
            {
                this.LoadCollectionSettingsForCollection(collection, collectionDocument);

                List<HurlSettingContainer> allSettingContainers =
                    collectionDocument.SettingSections.SelectMany(x => x.SettingContainers).ToList();
                this.EvaluateSettingInheritances(allSettingContainers);
            });
        }


        /// <summary>
        /// Consolidates a list of settings for a given folderDocument into the document
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="folderDocument"></param>
        private async Task LoadSettingsForFolder(HurlFolderContainer folder, FolderDocumentViewModel folderDocument)
        {
            if (folder == null) return;

            await Task.Run(() =>
            {
                this.LoadFolderSettingsForFolder(folder, folderDocument);

                List<HurlSettingContainer> allSettingContainers =
                    folderDocument.SettingSections.SelectMany(x => x.SettingContainers).ToList();
                this.EvaluateSettingInheritances(allSettingContainers);
            });
        }

        /// <summary>
        /// Creates a setting section for a folder container
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="folderDocument"></param>
        private void LoadFolderSettingsForFolder(HurlFolderContainer folder, FolderDocumentViewModel folderDocument)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);

            //folderDocument.SettingSections.Clear();
            folderDocument.SettingSections.RemoveAll(x => x.SectionType == HurlSettingSectionType.Folder);

            // Folder settings
            HurlSettingSection folderSection =
                new HurlSettingSection(folderDocument, HurlSettingSectionType.Folder, folder);
            folderSection.SettingContainers.AddRangeIfNotNull(
                folder.Folder?
                    .FolderSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x =>
                        new HurlSettingContainer(folderDocument, folderSection, x, false, true, EnableType.Setting)));

            folderDocument.SettingSections.Add(folderSection);

            // Bind events, without ui state interactions
            this.BindSettingContainers(folderSection.SettingContainers.ToList(), false);
        }

        /// <summary>
        /// Creates a setting section for a folder container
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="collectionDocument"></param>
        private void LoadCollectionSettingsForCollection(HurlCollectionContainer collection,
            CollectionDocumentViewModel collectionDocument)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);

            //folderDocument.SettingSections.Clear();
            collectionDocument.SettingSections.RemoveAll(x => x.SectionType == HurlSettingSectionType.Collection);

            // Folder settings
            HurlSettingSection collectionSection =
                new HurlSettingSection(collectionDocument, HurlSettingSectionType.Collection, collection);
            collectionSection.SettingContainers.AddRangeIfNotNull(
                collection.Collection?
                    .CollectionSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x =>
                        new HurlSettingContainer(collectionDocument, collectionSection, x, false, true,
                            EnableType.Setting)));

            collectionDocument.SettingSections.Add(collectionSection);

            // Bind events, without ui state interactions
            this.BindSettingContainers(collectionSection.SettingContainers.ToList(), false);
        }

        /// <summary>
        /// Collect the setting containers from their sections, set enabled/collapsed state and bind events
        /// </summary>
        /// <param name="settingContainers"></param>
        /// <param name="setUiState"></param>
        private void BindSettingContainers(List<HurlSettingContainer> settingContainers, bool setUiState)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);
            foreach (HurlSettingContainer hurlSettingContainer in settingContainers)
            {
                // Set collapse and enabled states if available from ui state
                if (uiState != null && setUiState)
                {
                    string settingContainerId = hurlSettingContainer.GetId();

                    if (uiState.SettingEnabledStates.ContainsKey(settingContainerId))
                    {
                        hurlSettingContainer.IsEnabled = uiState.SettingEnabledStates[settingContainerId];
                    }

                    if (uiState.SettingCollapsedStates.ContainsKey(settingContainerId))
                    {
                        hurlSettingContainer.Collapsed = uiState.SettingCollapsedStates[settingContainerId];
                    }
                }

                // Bind events for reevaluation
                hurlSettingContainer.SettingEnabledChanged += this.On_HurlSettingContainer_SettingEnabledChanged;
                hurlSettingContainer.SettingOrderChanged += this.On_HurlSettingContainer_SettingOrderChanged;
                hurlSettingContainer.SettingKeyChanged += this.On_HurlSettingContainer_SettingKeyChanged;
                hurlSettingContainer.SettingCollapsedChanged += this.On_HurlSettingContainer_SettingCollapsedChanged;
                hurlSettingContainer.SettingChanged += this.On_HurlSettingContainer_SettingChanged;
            }
        }

        /// <summary>
        /// On Section collapse, save the collapse state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_HurlSettingSection_SettingSectionCollapsedChanged(object? sender,
            Model.EventArgs.SettingSectionCollapsedChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingSection section)
            {
                _uiStateService.SetSettingSectionCollapsedState(section.GetId(), e.Collapsed);
            }
        }

        /// <summary>
        /// On setting container underlying setting property change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_HurlSettingContainer_SettingChanged(object? sender, Model.EventArgs.SettingChangedEventArgs e)
        {
            if (sender is HurlSettingContainer container)
            {
                if (_log.IsEnabled(LogLevel.Debug))
                {
                    _log.LogDebug(
                        $"Setting property changed: [{container.Document.GetId()}]: [{e.Setting.GetType().Name}]:[{e.Setting.GetConfigurationValue()}]");
                }

                container.Document.HasChanges = true;
            }
        }

        /// <summary>
        /// On setting collapsed state change -> set ui state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingCollapsedChanged(object? sender,
            Model.EventArgs.SettingCollapsedChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                _uiStateService.SetSettingCollapsedState(container.GetId(), e.Collapsed);
            }
        }

        /// <summary>
        /// On setting key change -> reevaluate inheritances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingKeyChanged(object? sender,
            Model.EventArgs.SettingEvaluationChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                await this.EvaluateSettingInheritancesAsync(container.Document);
            }
        }

        /// <summary>
        /// Reevaluate the overwritten flags on a setting being enabled or disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingEnabledChanged(object? sender,
            Model.EventArgs.SettingEnabledChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                // File settings are "active" via the file settings "#" prefix
                // Other types are "active" via the uiState, since we want enabling/disabling on a file basis
                if (container.EnableType == EnableType.Container)
                {
                    _uiStateService.SetSettingEnabledState(container.GetId(), e.Enabled);
                }

                List<HurlSettingContainer> allSettingContainers =
                    container.Document.SettingSections.SelectMany(x => x.SettingContainers).ToList();
                await Task.Run(() => { this.EvaluateSettingInheritances(allSettingContainers); });
            }
        }

        /// <summary>
        /// Apply sort value on OrderChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingOrderChanged(object? sender,
            Model.EventArgs.SettingOrderChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(false);
                OrderedObservableCollection<HurlSettingContainer>? source = container.SettingSection.SettingContainers;

                // Move item in observable collection and rebuild the ui state afterwards
                if (source != null && source.Contains(e.SettingContainer))
                {
                    e.SettingContainer.Document.HasChanges = true;

                    int settingIdx = source.IndexOf(e.SettingContainer);
                    List<HurlSettingContainer> affectedSettingContainers = new List<HurlSettingContainer>();

                    if (e.MoveDirection == MoveDirection.Up)
                    {
                        affectedSettingContainers.AddIfNotNull(e.SettingContainer);
                        affectedSettingContainers.AddIfNotNull(source.Get(settingIdx - 1));
                        source.MoveItemUp(e.SettingContainer);
                    }
                    else if (e.MoveDirection == MoveDirection.Down)
                    {
                        affectedSettingContainers.AddIfNotNull(e.SettingContainer);
                        affectedSettingContainers.AddIfNotNull(source.Get(settingIdx + 1));
                        source.MoveItemDown(e.SettingContainer);
                    }
                    else if (e.MoveDirection == MoveDirection.ToTop)
                    {
                        source.Remove(e.SettingContainer);
                        source.Insert(0, e.SettingContainer);
                        affectedSettingContainers.AddRange(source);
                    }
                    else if (e.MoveDirection == MoveDirection.ToBottom)
                    {
                        source.Remove(e.SettingContainer);
                        source.Add(e.SettingContainer);
                        affectedSettingContainers.AddRange(source);
                    }

                    // Re-build ui state with new ids
                    if (uiState != null)
                    {
                        foreach (HurlSettingContainer affectedSettingContainer in affectedSettingContainers)
                        {
                            string affectedSettingId = affectedSettingContainer.GetId();

                            _uiStateService.SetSettingCollapsedState(affectedSettingId,
                                affectedSettingContainer.Collapsed);
                        }
                    }
                }

                // Reevaluate inheritances
                await this.EvaluateSettingInheritancesAsync(container.Document);
            }
        }


        /// <summary>
        /// Reevaluates the inheritance types into 'Overwritten' flags
        /// </summary>
        /// <param name="fileDocumentViewModel"></param>
        /// <returns></returns>
        private async Task EvaluateSettingInheritancesAsync(IEditorDocument fileDocumentViewModel)
        {
            List<HurlSettingContainer> allSettingContainers =
                fileDocumentViewModel.SettingSections.SelectMany(x => x.SettingContainers).ToList();
            await Task.Run(() => this.EvaluateSettingInheritances(allSettingContainers));
        }

        /// <summary>
        /// Reevaluates the inheritance types into 'Overwritten' flags
        /// </summary>
        /// <param name="fileDocument"></param>
        private void EvaluateSettingInheritances(List<HurlSettingContainer> settingContainers)
        {
            var uiState = _uiStateService.GetUiState(false);
            settingContainers.ForEach(x => x.Overwritten = false);

            // Go through the list backwards 
            // since we want later entries to overwrite earlier ones
            // for (int i = allSettingContainers.Count - 1; i >= 0; i--)
            for (int i = 0; i < settingContainers.Count; i++)
            {
                HurlSettingContainer container = settingContainers[i];

                if (!container.Overwritten &&
                    ( // For File sections the files' enabled state is important, for other types it's the containers' enabled state 
                        (container.EnableType == EnableType.Setting && container.Setting.IsEnabled) ||
                        (container.EnableType == EnableType.Container && container.IsEnabled)
                    ) &&
                    container.Setting.GetInheritanceBehavior() != Common.Enums.HurlSettingInheritanceBehavior.Append)
                {
                    // Go over the preceding settings to find matching ones to be overwritten
                    // for (int j = i - 1; j >= 0; j--)
                    for (int j = i + 1; j < settingContainers.Count; j++)
                    {
                        // Same type on overwrite setting -> set all earlier settings of this type to overwritten
                        if (container.Setting.GetInheritanceBehavior() ==
                            Common.Enums.HurlSettingInheritanceBehavior.Overwrite &&
                            settingContainers[j].Setting.GetType() == container.Setting.GetType())
                        {
                            settingContainers[j].Overwritten = true;
                        }
                        // Same type on unique key setting -> compare configuration key, if matching set container to overwritten
                        else if (container.Setting.GetInheritanceBehavior() ==
                                 Common.Enums.HurlSettingInheritanceBehavior.UniqueKey &&
                                 settingContainers[j].Setting.GetType() == container.Setting.GetType() &&
                                 settingContainers[j].Setting.GetConfigurationKey() ==
                                 container.Setting.GetConfigurationKey())
                        {
                            settingContainers[j].Overwritten = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fills a folder document viewmodel with settings
        /// </summary>
        /// <param name="folderLocation"></param>
        /// <param name="collectionLocation"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task SetFolderDocument(FolderDocumentViewModel folderDocument, string folderLocation,
            string collectionLocation)
        {
            // Open collection 
            if (!File.Exists(collectionLocation)) throw new IOException($"{collectionLocation} does not exist");

            HurlCollection collection = await _collectionService.GetCollectionAsync(collectionLocation);
            HurlCollectionContainer collectionContainer =
                await _collectionService.GetCollectionContainerAsync(collection);
            HurlFolderContainer? folder = this.GetAllFoldersFromCollection(collectionContainer)
                .Where(x => x.AbsoluteLocation == folderLocation).FirstOrDefault();

            if (folder == null) throw new ArgumentNullException(nameof(folder));
            folderDocument.Id = folder.GetId();
            folderDocument.FolderContainer = folder;

            // Load settings
            await this.LoadSettingsForFolder(folder, folderDocument);
        }

        /// <summary>
        /// Fills an environment document viewmodel with settings
        /// </summary>
        /// <param name="environmentLocation"></param>
        /// <param name="environmentDocument"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task SetEnvironmentDocument(EnvironmentDocumentViewModel environmentDocument,
            string environmentLocation)
        {
            // Open collection 
            if (!File.Exists(environmentLocation)) throw new IOException($"{environmentLocation} does not exist");

            HurlEnvironment environment = await _environmentService.GetEnvironmentAsync(environmentLocation);
            HurlEnvironmentContainer? environmentContainer = null;
            if (environmentDocument.EnvironmentContainer != null)
            {
                environmentContainer =
                    await _environmentService.SetEnvironmentContainerAsync(environmentDocument.EnvironmentContainer,
                        environment);
            }
            else
            {
                environmentContainer = await _environmentService.GetEnvironmentContainerAsync(environment);
            }

            if (environmentContainer == null) throw new ArgumentNullException(nameof(environmentContainer));
            environmentDocument.Id = environmentContainer.GetId();
            environmentDocument.EnvironmentContainer = environmentContainer;

            // Load settings
            await this.LoadSettingsForEnvironment(environmentContainer, environmentDocument);
        }


        /// <summary>
        /// Consolidates a list of settings for a given environmentDocument into the document
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="environmentDocument"></param>
        private async Task LoadSettingsForEnvironment(HurlEnvironmentContainer environment,
            EnvironmentDocumentViewModel environmentDocument)
        {
            if (environment == null) return;

            await Task.Run(() =>
            {
                this.LoadEnvironmentSettingsForEnvironment(environment, environmentDocument);

                List<HurlSettingContainer> allSettingContainers =
                    environmentDocument.SettingSections.SelectMany(x => x.SettingContainers).ToList();
                this.EvaluateSettingInheritances(allSettingContainers);
            });
        }

        /// <summary>
        /// Creates a setting section for an environment container
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="environmentDocument"></param>
        private void LoadEnvironmentSettingsForEnvironment(HurlEnvironmentContainer environment,
            EnvironmentDocumentViewModel environmentDocument)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);

            environmentDocument.SettingSections.RemoveAll(x => x.SectionType == HurlSettingSectionType.Environment);

            // Environment settings
            HurlSettingSection environmentSettingSection =
                new HurlSettingSection(environmentDocument, HurlSettingSectionType.Environment, environment);
            environmentSettingSection.SettingContainers.AddRangeIfNotNull(
                environment.Environment?
                    .EnvironmentSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x => new HurlSettingContainer(environmentDocument, environmentSettingSection, x, false,
                        true, EnableType.Setting)));

            environmentDocument.SettingSections.Add(environmentSettingSection);

            // Bind events, without ui state interactions
            this.BindSettingContainers(environmentSettingSection.SettingContainers.ToList(), false);
        }

        /// <summary>
        /// Close welcome document if configured
        /// </summary>
        /// <returns></returns>
        private async Task CloseWelcomeDocument()
        {
            // Get user settings
            Model.UserSettings.UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);
            // Close welcome document
            if (userSettings.CloseWelcomeDocumentOnFileOpen)
            {
                IDockable? welcomeDocument =
                    _editorViewViewModel.Documents.FirstOrDefault(x => x is WelcomeDocumentViewModel);
                if (welcomeDocument != null)
                {
                    _layoutFactory.CloseDockable(welcomeDocument);
                }
            }
        }

        /// <summary>
        /// Fills a collection document viewmodel with settings
        /// </summary>
        /// <param name="collectionDocument"></param>
        /// <param name="collectionLocation"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task SetCollectionDocument(CollectionDocumentViewModel collectionDocument,
            string collectionLocation)
        {
            // Open collection 
            if (!File.Exists(collectionLocation)) throw new IOException($"{collectionLocation} does not exist");

            HurlCollection collection = await _collectionService.GetCollectionAsync(collectionLocation);
            HurlCollectionContainer? collectionContainer;
            if (collectionDocument.CollectionContainer != null)
            {
                collectionContainer =
                    await _collectionService.SetCollectionContainerAsync(collectionDocument.CollectionContainer,
                        collection);
            }
            else
            {
                collectionContainer = await _collectionService.GetCollectionContainerAsync(collection);
            }

            if (collectionContainer == null) throw new ArgumentNullException(nameof(collectionContainer));
            collectionDocument.Id = collectionContainer.GetId();
            collectionDocument.CollectionContainer = collectionContainer;

            // Load settings
            await this.LoadSettingsForCollection(collectionContainer, collectionDocument);
        }

        /// <summary>
        /// Collects all files in a collection
        /// </summary>
        /// <param name="collectionContainer"></param>
        /// <returns></returns>
        private List<HurlFileContainer> GetAllFilesFromCollection(HurlCollectionContainer collectionContainer)
        {
            List<HurlFileContainer> files = new List<HurlFileContainer>();

            files.AddRange(collectionContainer.Files);
            foreach (HurlFolderContainer subFolder in collectionContainer.Folders)
            {
                files.AddRange(this.GetAllFilesFromFolder(subFolder));
            }

            return files;
        }

        /// <summary>
        /// Collects all files in a collection
        /// </summary>
        /// <param name="collectionContainer"></param>
        /// <returns></returns>
        private List<HurlFolderContainer> GetAllFoldersFromCollection(HurlCollectionContainer collectionContainer)
        {
            List<HurlFolderContainer> files = new List<HurlFolderContainer>();

            files.AddRange(collectionContainer.Folders);
            foreach (HurlFolderContainer subFolder in collectionContainer.Folders)
            {
                files.AddRange(this.GetAllFoldersFromFolder(subFolder));
            }

            return files;
        }

        /// <summary>
        /// Recursiveley collects all files from a given folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private List<HurlFileContainer> GetAllFilesFromFolder(HurlFolderContainer folder)
        {
            List<HurlFileContainer> files = new List<HurlFileContainer>();

            files.AddRange(folder.Files);
            foreach (HurlFolderContainer subFolder in folder.Folders)
            {
                files.AddRange(this.GetAllFilesFromFolder(subFolder));
            }

            return files;
        }

        /// <summary>
        /// Recursiveley collects all files from a given folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private List<HurlFolderContainer> GetAllFoldersFromFolder(HurlFolderContainer folder)
        {
            List<HurlFolderContainer> folders = new List<HurlFolderContainer>();

            folders.AddRange(folder.Folders);
            foreach (HurlFolderContainer subFolder in folder.Folders)
            {
                folders.AddRange(this.GetAllFoldersFromFolder(subFolder));
            }

            return folders;
        }

        /// <summary>
        /// Returns a list of folders in the hierarchy
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private List<HurlFolderContainer> GetParentFolders(HurlFolderContainer folder)
        {
            List<HurlFolderContainer> collectionFolders = new List<HurlFolderContainer>();

            if (folder.ParentFolderContainer != null)
            {
                collectionFolders.AddRangeIfNotNull(this.GetParentFolders(folder.ParentFolderContainer));
            }

            collectionFolders.Add(folder);

            return collectionFolders;
        }

        /// <summary>
        /// Returns the FileDocument for the given collection fileDocument
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private FileDocumentViewModel? GetFileDocumentByAbsoluteFilePath(string fileLocation)
        {
            FileDocumentViewModel? openDocument = _editorViewViewModel.Documents.Where(x => x is FileDocumentViewModel)
                .Select(x => (FileDocumentViewModel)x)
                .Where(x => x.FileContainer != null && x.FileContainer.AbsoluteLocation.Equals(fileLocation))
                .FirstOrDefault();

            return openDocument;
        }

        /// <summary>
        /// Returns the FolderDocument for the given collection folder container
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private FolderDocumentViewModel? GetFolderDocumentByAbsoluteFolderLocation(string folderLocation)
        {
            FolderDocumentViewModel? openDocument = _editorViewViewModel.Documents
                .Where(x => x is FolderDocumentViewModel)
                .Select(x => (FolderDocumentViewModel)x)
                .Where(x => x.FolderContainer != null && x.FolderContainer.AbsoluteLocation.Equals(folderLocation))
                .FirstOrDefault();

            return openDocument;
        }

        /// <summary>
        /// Returns the FolderDocument for the given collection folder container
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private CollectionDocumentViewModel? GetCollectionDocumentByAbsoluteCollectionLocation(
            string collectionLocation)
        {
            CollectionDocumentViewModel? openDocument = _editorViewViewModel.Documents
                .Where(x => x is CollectionDocumentViewModel)
                .Select(x => (CollectionDocumentViewModel)x)
                .Where(x => x.CollectionContainer != null &&
                            x.CollectionContainer.Collection.CollectionFileLocation.Equals(collectionLocation))
                .FirstOrDefault();

            return openDocument;
        }

        /// <summary>
        /// Returns the EnvironmentDocument for the given environment document
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private EnvironmentDocumentViewModel? GetEnvironmentDocumentByAbsoluteEnvironmentLocation(
            string environmentLocation)
        {
            EnvironmentDocumentViewModel? openDocument = _editorViewViewModel.Documents
                .Where(x => x is EnvironmentDocumentViewModel)
                .Select(x => (EnvironmentDocumentViewModel)x)
                .Where(x => x.EnvironmentContainer != null &&
                            x.EnvironmentContainer.EnvironmentFileLocation.Equals(environmentLocation))
                .FirstOrDefault();

            return openDocument;
        }
    }
}