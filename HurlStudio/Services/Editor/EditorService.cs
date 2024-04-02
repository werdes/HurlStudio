using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Common.Extensions;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI;
using HurlStudio.UI.Controls.CollectionExplorer;
using HurlStudio.UI.Controls.Documents;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using HurlStudio.Common;
using AvaloniaEdit;
using HurlStudio.Collections.Settings;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Services.Notifications;
using HurlStudio.Common.UI;
using HurlStudio.Services.UiState;
using HurlStudio.Model.UiState;
using HurlStudio.UI.Controls;

namespace HurlStudio.Services.Editor
{
    public class EditorService : IEditorService
    {
        private ILogger _log;
        private LayoutFactory _layoutFactory;
        private ServiceManager<Document> _documentControlBuilder;
        private EditorViewViewModel _editorViewViewModel;
        private MainViewViewModel _mainViewViewModel;
        private INotificationService _notificationService;
        private IUserSettingsService _userSettingsService;
        private IUiStateService _uiStateService;
        private IConfiguration _configuration;

        private int _fileHistoryLength = 0;

        public EditorService(ILogger<EditorService> logger, ServiceManager<Document> documentControlBuilder, EditorViewViewModel editorViewViewModel, MainViewViewModel mainViewViewModel, LayoutFactory layoutFactory, IUserSettingsService userSettingsService, INotificationService notificationService, IConfiguration configuration, IUiStateService uiStateService)
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

            _fileHistoryLength = Math.Max(_configuration.GetValue<int>("fileHistoryLength"), GlobalConstants.DEFAULT_FILE_HISTORY_LENGTH);
        }

        public async Task<ObservableCollection<IDockable>> GetOpenDocuments()
        {
            ObservableCollection<IDockable> dockables = new ObservableCollection<IDockable>();
            dockables.Add(_documentControlBuilder.Get<WelcomeDocumentViewModel>());

            return dockables;
        }

        public Task<bool> MoveFileToCollection(CollectionFile collectionFile, CollectionFolder parentFolder, CollectionContainer collection)
        {
            _log.LogInformation($"Moving [{collectionFile}] to [{collection}], folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFileToCollectionRoot(CollectionFile collectionFile, CollectionContainer collection)
        {
            _log.LogInformation($"Moving [{collectionFile}] to [{collection}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFileToFolder(CollectionFile collectionFile, CollectionFolder folder)
        {
            _log.LogInformation($"Moving [{collectionFile}] to folder [{folder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToCollection(CollectionFolder folder, CollectionFolder parentFolder, CollectionContainer collection)
        {
            _log.LogInformation($"Moving [{folder}] to [{collection}], folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToCollectionRoot(CollectionFolder folder, CollectionContainer collection)
        {
            _log.LogInformation($"Moving [{folder}] to collection [{collection}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToFolder(CollectionFolder folder, CollectionFolder parentFolder)
        {
            _log.LogInformation($"Moving [{folder}] to folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task OpenCollectionSettings(CollectionContainer collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a collection fileDocument as a document 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task OpenFile(CollectionFile file)
        {
            if (_editorViewViewModel.Layout == null) throw new ArgumentNullException(nameof(_editorViewViewModel.Layout));

            try
            {
                _mainViewViewModel.StatusBarStatus = Model.Enums.StatusBarStatus.OpeningFile;
                _mainViewViewModel.StatusBarDetail = file.Location;

                IDockable? openDocument = this.GetFileDocumentByFile(file);

                // Document isn't opened in the dock already
                if (openDocument == null)
                {
                    FileDocumentViewModel fileDocument = _documentControlBuilder.Get<FileDocumentViewModel>();
                    fileDocument.Id = file.GetId();
                    fileDocument.File = file;
                    fileDocument.Document = new AvaloniaEdit.Document.TextDocument(await System.IO.File.ReadAllTextAsync(file.Location, Encoding.UTF8));

                    // Load settings
                    await Task.Run(() => this.LoadSettingsForFile(file, fileDocument));

                    // Open in dock
                    _layoutFactory.AddDocument(fileDocument);
                    _layoutFactory.SetActiveDockable(fileDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, fileDocument);


                    // Place fileDocument in history
                    _editorViewViewModel.FileHistoryEntries.RemoveAll(x => x.Location == file.Location);
                    _editorViewViewModel.FileHistoryEntries.Add(new FileHistoryEntry(file.Location, DateTime.UtcNow));
                    _editorViewViewModel.FileHistoryEntries = new ObservableCollection<FileHistoryEntry>(_editorViewViewModel.FileHistoryEntries.OrderByDescending(x => x.LastOpened).Take(_fileHistoryLength));
                }
                else
                {
                    _layoutFactory.SetActiveDockable(openDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, openDocument);
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, nameof(OpenFile));
                _notificationService.Notify(ex);

                throw new Exception($"Opening file [{file.Location}] failed", ex);
            }
            finally
            {
                _mainViewViewModel.StatusBarStatus = Model.Enums.StatusBarStatus.Idle;
                _mainViewViewModel.StatusBarDetail = string.Empty;
            }
        }

        /// <summary>
        /// Consolidates a list of settings for a given fileDocument into the document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileDocument"></param>
        private void LoadSettingsForFile(CollectionFile file, FileDocumentViewModel fileDocument)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);

            fileDocument.SettingSections.Clear();

            // Environment settings
            HurlSettingSection environmentSection = new HurlSettingSection(Model.Enums.HurlSettingSectionType.Environment, _editorViewViewModel.ActiveEnvironment);
            environmentSection.SettingContainers.AddRangeIfNotNull(
                _editorViewViewModel
                    .ActiveEnvironment?
                    .Environment
                    .Settings
                        .Where(x => x is BaseSetting)
                        .Select(x => (BaseSetting)x)
                        .Select(x => 
                            new HurlSettingContainer(fileDocument, environmentSection, x, true)
                        )
            );
            fileDocument.SettingSections.Add(environmentSection);

            // Collection settings
            HurlSettingSection collectionSection = new HurlSettingSection(Model.Enums.HurlSettingSectionType.Collection, file.Collection);
            collectionSection.SettingContainers.AddRangeIfNotNull(
                file.Collection
                    .Collection
                    .CollectionSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x => new HurlSettingContainer(fileDocument, collectionSection, x, true)));
            fileDocument.SettingSections.Add(collectionSection);

            // Folder settings
            List<CollectionFolder> collectionFolders = this.GetParentFolders(file.Folder);
            foreach (CollectionFolder folder in collectionFolders)
            {
                HurlSettingSection folderSection = new HurlSettingSection(Model.Enums.HurlSettingSectionType.Folder, folder);
                folderSection.SettingContainers.AddRangeIfNotNull(
                    folder.Folder?
                          .FolderSettings
                          .Where(x => x is BaseSetting)
                          .Select(x => (BaseSetting)x)
                          .Select(x => new HurlSettingContainer(fileDocument, folderSection, x, true)));

                fileDocument.SettingSections.Add(folderSection);
            }

            // File settings
            HurlSettingSection fileSection = new HurlSettingSection(Model.Enums.HurlSettingSectionType.File, file);
            fileSection.SettingContainers.AddRangeIfNotNull(
                file.File?
                    .FileSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x => new HurlSettingContainer(fileDocument, fileSection, x, false)));
            fileDocument.SettingSections.Add(fileSection);


            // Inheritance behavior
            List<HurlSettingContainer> allSettingContainers = fileDocument.SettingSections.SelectMany(x => x.SettingContainers).ToList();

            foreach (HurlSettingContainer hurlSettingContainer in allSettingContainers)
            {
                // Set Enabled states if available from ui state
                if (uiState != null)
                {
                    string settingContainerId = hurlSettingContainer.GetId();
                    if (uiState.SettingEnabledStates.ContainsKey(settingContainerId))
                    {
                        hurlSettingContainer.Enabled = uiState.SettingEnabledStates[settingContainerId];
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
            }

            this.EvaluateInheritances(fileDocument);
        }

        /// <summary>
        /// On setting collapsed state change -> set ui state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingCollapsedChanged(object? sender, Model.EventArgs.SettingCollapsedChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                await Task.Run(() =>
                {
                    _uiStateService.SetSettingCollapsedState(container.GetId(), e.Collapsed);
                });
            }
        }

        /// <summary>
        /// On setting key change -> reevaluate inheritances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingKeyChanged(object? sender, Model.EventArgs.SettingKeyChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                await Task.Run(() => this.EvaluateInheritances(container.Document));
            }
        }

        /// <summary>
        /// Reevaluate the overwritten flags on a setting being enabled or disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingEnabledChanged(object? sender, Model.EventArgs.SettingEnabledChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                await Task.Run(() =>
                {
                    this.EvaluateInheritances(container.Document);
                    _uiStateService.SetSettingEnabledState(container.GetId(), e.Enabled);
                });
            }
        }

        /// <summary>
        /// Apply sort value on OrderChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingOrderChanged(object? sender, Model.EventArgs.SettingOrderChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(false);
                OrderedObservableCollection<HurlSettingContainer>? source = container.Section.SettingContainers;

                // Move item in observable collection and rebuild the ui state afterwards
                if (source != null && source.Contains(e.Setting))
                {
                    int settingIdx = source.IndexOf(e.Setting);
                    HurlSettingContainer? otherSettingContainer = null;

                    if (e.MoveDirection == Model.Enums.MoveDirection.Up)
                    {
                        otherSettingContainer = source.Get(settingIdx - 1);
                        source.MoveItemUp(e.Setting);
                    }
                    else if (e.MoveDirection == Model.Enums.MoveDirection.Down)
                    {
                        otherSettingContainer = source.Get(settingIdx + 1);
                        source.MoveItemDown(e.Setting);
                    }

                    // Re-build ui state with new ids
                    if (uiState != null)
                    {
                        string settingId = e.Setting.GetId();
                        _uiStateService.SetSettingCollapsedState(settingId, e.Setting.Collapsed);
                        _uiStateService.SetSettingEnabledState(settingId, e.Setting.Enabled);

                        if (otherSettingContainer != null)
                        {
                            string otherSettingId = otherSettingContainer.GetId();

                            _uiStateService.SetSettingCollapsedState(otherSettingId, otherSettingContainer.Collapsed);
                            _uiStateService.SetSettingEnabledState(otherSettingId, otherSettingContainer.Enabled);
                        }

                    }
                }

                // Reevaluate inheritances
                await Task.Run(() => this.EvaluateInheritances(container.Document));
            }
        }

        /// <summary>
        /// Reevaluates the inheritance types into 'Overwritten' flags
        /// </summary>
        /// <param name="fileDocument"></param>
        private void EvaluateInheritances(FileDocumentViewModel fileDocument)
        {
            var uiState = _uiStateService.GetUiState(false);

            List<HurlSettingContainer> allSettingContainers = fileDocument.SettingSections.SelectMany(x => x.SettingContainers).ToList();


            allSettingContainers.ForEach(x => x.Overwritten = false);

            // Go through the list backwards 
            // since we want later entries to overwrite earlier ones
            for (int i = allSettingContainers.Count - 1; i >= 0; i--)
            {
                HurlSettingContainer container = allSettingContainers[i];

                if (!container.Overwritten &&
                    container.Enabled &&
                    container.Setting.GetInheritanceBehavior() != Common.Enums.HurlSettingInheritanceBehavior.Append)
                {
                    // Go over the preceding settings to find matching ones to be overwritten
                    for (int j = i - 1; j >= 0; j--)
                    {
                        // Same type on overwrite setting -> set all earlier settings of this type to overwritten
                        if (container.Setting.GetInheritanceBehavior() == Common.Enums.HurlSettingInheritanceBehavior.Overwrite &&
                            allSettingContainers[j].Setting.GetType() == container.Setting.GetType())
                        {
                            allSettingContainers[j].Overwritten = true;
                        }
                        // Same type on unique key setting -> compare configuration key, if matching set container to overwritten
                        else if (container.Setting.GetInheritanceBehavior() == Common.Enums.HurlSettingInheritanceBehavior.UniqueKey &&
                                 allSettingContainers[j].Setting.GetType() == container.Setting.GetType() &&
                                 allSettingContainers[j].Setting.GetConfigurationKey() == container.Setting.GetConfigurationKey())
                        {
                            allSettingContainers[j].Overwritten = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Opens a fileDocument from just its path
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public async Task OpenFile(string fileLocation)
        {
            CollectionFile? collectionFile = _editorViewViewModel.Collections.SelectMany(x => this.GetAllFilesFromCollection(x)).Where(x => x.Location == fileLocation).FirstOrDefault();

            if (collectionFile != null)
            {
                await this.OpenFile(collectionFile);
            }
        }

        /// <summary>
        /// Close a file
        /// -> properly unbind all setting events
        /// </summary>
        /// <param name="fileDocument"></param>
        /// <returns></returns>
        public async Task CloseFileDocument(FileDocumentViewModel? fileDocument)
        {
            if (fileDocument != null)
            {
                List<HurlSettingContainer> allSettingContainers = fileDocument.SettingSections.SelectMany(x => x.SettingContainers).ToList();

                foreach (HurlSettingContainer hurlSettingContainer in allSettingContainers)
                {
                    hurlSettingContainer.SettingEnabledChanged -= this.On_HurlSettingContainer_SettingEnabledChanged;
                    hurlSettingContainer.SettingOrderChanged -= this.On_HurlSettingContainer_SettingOrderChanged;
                    hurlSettingContainer.SettingKeyChanged -= this.On_HurlSettingContainer_SettingKeyChanged;
                }
            }

        }

        /// <summary>
        /// Opens a folder settings document
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task OpenFolderSettings(CollectionFolder folder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Collects all files in a collection
        /// </summary>
        /// <param name="collectionContainer"></param>
        /// <returns></returns>
        private List<CollectionFile> GetAllFilesFromCollection(CollectionContainer collectionContainer)
        {
            List<CollectionFile> files = new List<CollectionFile>();

            files.AddRange(collectionContainer.Files);
            foreach (CollectionFolder subFolder in collectionContainer.Folders)
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
        private List<CollectionFile> GetAllFilesFromFolder(CollectionFolder folder)
        {
            List<CollectionFile> files = new List<CollectionFile>();

            files.AddRange(folder.Files);
            foreach (CollectionFolder subFolder in folder.Folders)
            {
                files.AddRange(this.GetAllFilesFromFolder(subFolder));
            }

            return files;
        }

        /// <summary>
        /// Returns a list of folders in the hierarchy
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private List<CollectionFolder> GetParentFolders(CollectionFolder folder)
        {
            List<CollectionFolder> collectionFolders = new List<CollectionFolder>();

            if (folder.ParentFolder != null)
            {
                collectionFolders.AddRangeIfNotNull(this.GetParentFolders(folder.ParentFolder));
            }
            collectionFolders.Add(folder);

            return collectionFolders;
        }

        /// <summary>
        /// Returns the FileDocument for the given collection fileDocument
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private FileDocumentViewModel? GetFileDocumentByFile(CollectionFile file)
        {
            FileDocumentViewModel? openDocument = _editorViewViewModel.Documents.Where(x => x is FileDocumentViewModel)
                                                                                .Select(x => (FileDocumentViewModel)x)
                                                                                .Where(x => x.File != null && x.File.Equals(file))
                                                                                .FirstOrDefault();

            return openDocument;
        }
    }
}
