using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.HurlContainers;
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
using HurlStudio.Collections.Model.Collection;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using HurlStudio.UI.Localization;
using MsBox.Avalonia.Models;
using System.Threading;
using Avalonia.Controls;
using HurlStudio.Collections.Utility;
using HurlStudio.Utility;
using HurlStudio.Collections.Model.Serializer;
using System.Collections.Immutable;
using System.Reflection;
using HurlStudio.Model.UserSettings;

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
        private ICollectionService _collectionService;

        private SemaphoreLock _lock = new SemaphoreLock();
        private int _fileHistoryLength = 0;


        public EditorService(ILogger<EditorService> logger, ServiceManager<Document> documentControlBuilder, EditorViewViewModel editorViewViewModel, MainViewViewModel mainViewViewModel, LayoutFactory layoutFactory, IUserSettingsService userSettingsService, INotificationService notificationService, IConfiguration configuration, IUiStateService uiStateService, ICollectionService collectionService)
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

            _fileHistoryLength = Math.Max(_configuration.GetValue<int>("fileHistoryLength"), GlobalConstants.DEFAULT_FILE_HISTORY_LENGTH);
        }

        /// <summary>
        /// Opens the open documents of the previous session
        /// </summary>
        /// <returns></returns>
        public async Task OpenInitialDocuments()
        {
            _layoutFactory.AddDocument(_documentControlBuilder.Get<WelcomeDocumentViewModel>());

            // Open files from previous session
            Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(false);
            if (uiState != null)
            {
                foreach (string file in uiState.OpenedFiles)
                {
                    await this.OpenFile(file);
                }
            }
        }

        /// <summary>
        /// Loads the initial Usersettings for the editor view model
        /// </summary>
        /// <returns></returns>
        public async Task LoadInitialUserSettings()
        {
            Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(false);
            Model.UserSettings.UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);

            _editorViewViewModel.FileHistoryEntries.AddRangeIfNotNull(uiState?.FileHistoryEntries);
            _editorViewViewModel.ShowWhitespace = userSettings.ShowWhitespace;
            _editorViewViewModel.ShowEndOfLine = userSettings.ShowEndOfLine;
            _editorViewViewModel.WordWrap = userSettings.WordWrap;
        }

        public Task<bool> MoveFileToCollection(HurlFileContainer collectionFile, HurlFolderContainer parentFolder, HurlCollectionContainer collection)
        {
            _log.LogInformation($"Moving [{collectionFile}] to [{collection}], folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFileToCollectionRoot(HurlFileContainer collectionFile, HurlCollectionContainer collection)
        {
            _log.LogInformation($"Moving [{collectionFile}] to [{collection}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFileToFolder(HurlFileContainer collectionFile, HurlFolderContainer folder)
        {
            _log.LogInformation($"Moving [{collectionFile}] to folder [{folder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToCollection(HurlFolderContainer folder, HurlFolderContainer parentFolder, HurlCollectionContainer collection)
        {
            _log.LogInformation($"Moving [{folder}] to [{collection}], folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToCollectionRoot(HurlFolderContainer folder, HurlCollectionContainer collection)
        {
            _log.LogInformation($"Moving [{folder}] to collection [{collection}]");
            throw new NotImplementedException();
        }

        public Task<bool> MoveFolderToFolder(HurlFolderContainer folder, HurlFolderContainer parentFolder)
        {
            _log.LogInformation($"Moving [{folder}] to folder [{parentFolder}]");
            throw new NotImplementedException();
        }

        public Task OpenCollectionSettings(HurlCollectionContainer collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a collection fileDocument as a document 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task OpenFile(string fileLocation, string collectionLocation)
        {
            if (_editorViewViewModel.Layout == null) throw new ArgumentNullException(nameof(_editorViewViewModel.Layout));

            try
            {
                // Get user settings
                Model.UserSettings.UserSettings userSettings = await _userSettingsService.GetUserSettingsAsync(false);

                // Open collection 
                if (!System.IO.File.Exists(fileLocation)) throw new IOException($"{fileLocation} does not exist");
                if (!System.IO.File.Exists(collectionLocation)) throw new IOException($"{fileLocation} does not exist");

                HurlCollection collection = await _collectionService.GetCollectionAsync(collectionLocation);
                HurlCollectionContainer collectionContainer = await _collectionService.GetCollectionContainerAsync(collection);
                HurlFileContainer? file = this.GetAllFilesFromCollection(collectionContainer).Where(x => x.Location == fileLocation).FirstOrDefault();

                if (file == null) throw new ArgumentNullException(nameof(file));


                _mainViewViewModel.StatusBarStatus = Model.Enums.StatusBarStatus.OpeningFile;
                _mainViewViewModel.StatusBarDetail = fileLocation;

                IDockable? openDocument = this.GetFileDocumentByFile(file);

                // Document isn't opened in the dock already
                if (openDocument == null)
                {
                    FileDocumentViewModel fileDocument = _documentControlBuilder.Get<FileDocumentViewModel>();
                    fileDocument.Id = file.GetId();
                    fileDocument.File = file;
                    fileDocument.Document = new AvaloniaEdit.Document.TextDocument(await System.IO.File.ReadAllTextAsync(file.Location, Encoding.UTF8));
                    fileDocument.SettingAdded += this.On_FileDocument_SettingAdded;
                    fileDocument.SettingRemoved += this.On_FileDocument_SettingRemoved;

                    // Load settings
                    await Task.Run(() => 
                    {
                        this.LoadInheritedSettings(file, fileDocument);
                        this.LoadFileSettings(file, fileDocument);
                        this.EvaluateSettingInheritances(fileDocument);
                    });

                    // Open in dock
                    _layoutFactory.AddDocument(fileDocument);
                    _layoutFactory.SetActiveDockable(fileDocument);
                    _layoutFactory.SetFocusedDockable(_editorViewViewModel.Layout, fileDocument);

                    // Close welcome document
                    if (userSettings.CloseWelcomeDocumentOnFileOpen)
                    {
                        IDockable? welcomeDocument = _editorViewViewModel.Documents.FirstOrDefault(x => x is WelcomeDocumentViewModel);
                        if (welcomeDocument != null)
                        {
                            _layoutFactory.CloseDockable(welcomeDocument);
                        }
                    }

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

                throw new Exception($"Opening file [{fileLocation}] failed", ex);
            }
            finally
            {
                _mainViewViewModel.StatusBarStatus = Model.Enums.StatusBarStatus.Idle;
                _mainViewViewModel.StatusBarDetail = string.Empty;
            }
        }

        /// <summary>
        /// Reevaluate settings when one is removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_FileDocument_SettingRemoved(object? sender, Model.EventArgs.SettingEvaluationChangedEventArgs e)
        {
            if (sender != null && sender is FileDocumentViewModel document)
            {
                // Unbind events for reevaluation
                e.Setting.SettingEnabledChanged -= this.On_HurlSettingContainer_SettingEnabledChanged;
                e.Setting.SettingOrderChanged -= this.On_HurlSettingContainer_SettingOrderChanged;
                e.Setting.SettingKeyChanged -= this.On_HurlSettingContainer_SettingKeyChanged;
                e.Setting.SettingCollapsedChanged -= this.On_HurlSettingContainer_SettingCollapsedChanged;
                e.Setting.SettingChanged -= this.On_HurlSettingContainer_SettingChanged;


                await Task.Run(() => this.EvaluateSettingInheritances(document));
            }
        }

        /// <summary>
        /// Reevaluate settings when a new one is added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_FileDocument_SettingAdded(object? sender, Model.EventArgs.SettingEvaluationChangedEventArgs e)
        {
            if (sender != null && sender is FileDocumentViewModel document)
            {
                // Bind events for reevaluation
                e.Setting.SettingEnabledChanged += this.On_HurlSettingContainer_SettingEnabledChanged;
                e.Setting.SettingOrderChanged += this.On_HurlSettingContainer_SettingOrderChanged;
                e.Setting.SettingKeyChanged += this.On_HurlSettingContainer_SettingKeyChanged;
                e.Setting.SettingCollapsedChanged += this.On_HurlSettingContainer_SettingCollapsedChanged;
                e.Setting.SettingChanged += this.On_HurlSettingContainer_SettingChanged;

                await Task.Run(() => this.EvaluateSettingInheritances(document));
            }
        }

        /// <summary>
        /// Tries to find a corresponding collection to a file and opens it
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public async Task OpenFile(string fileLocation)
        {
            HurlFileContainer? collectionFile = _editorViewViewModel.Collections.SelectMany(x => this.GetAllFilesFromCollection(x)).Where(x => x.Location == fileLocation).FirstOrDefault();

            if (collectionFile != null &&
                collectionFile.Collection != null &&
                collectionFile.Collection.Collection != null &&
                collectionFile.Collection.Collection.FileLocation != null)
            {
                await this.OpenFile(fileLocation, collectionFile.Collection.Collection.FileLocation);
            }
        }

        /// <summary>
        /// Consolidates a list of settings for a given fileDocument into the document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileDocument"></param>
        private void LoadFileSettings(HurlFileContainer file, FileDocumentViewModel fileDocument)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);

            fileDocument.SettingSections.RemoveAll(x => x.SectionType == Model.Enums.HurlSettingSectionType.File);

            // File settings
            HurlSettingSection fileSection = new HurlSettingSection(fileDocument, Model.Enums.HurlSettingSectionType.File, file);
            fileSection.SettingContainers.AddRangeIfNotNull(
                file.File?
                    .FileSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x => new HurlSettingContainer(fileDocument, fileSection, x, false, true)));
            fileDocument.SettingSections.Add(fileSection);
            

            // Apply sections' ui state and bind relevant events
            string sectionId = fileSection.GetId();
            if (uiState != null && uiState.SettingSectionCollapsedStates.ContainsKey(sectionId))
            {
                fileSection.Collapsed = uiState.SettingSectionCollapsedStates[sectionId];
            }

            fileSection.SettingSectionCollapsedChanged -= this.On_HurlSettingSection_SettingSectionCollapsedChanged;
            fileSection.SettingSectionCollapsedChanged += this.On_HurlSettingSection_SettingSectionCollapsedChanged;
            
            // Inheritance behavior
            this.BindSettingContainers(fileSection.SettingContainers.ToList());
        }

        /// <summary>
        /// Consolidates a list of settings for a given fileDocument into the document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileDocument"></param>
        private void LoadInheritedSettings(HurlFileContainer file, FileDocumentViewModel fileDocument)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);
            List<HurlSettingSection> sections = new List<HurlSettingSection>();

            fileDocument.SettingSections.RemoveAll(x => x.SectionType != Model.Enums.HurlSettingSectionType.File);
            int sectionIdx = 0;

            // Environment settings
            HurlSettingSection environmentSection = new HurlSettingSection(fileDocument, Model.Enums.HurlSettingSectionType.Environment, _editorViewViewModel.ActiveEnvironment);
            environmentSection.SettingContainers.AddRangeIfNotNull(
                _editorViewViewModel
                    .ActiveEnvironment?
                    .Environment
                    .Settings
                        .Where(x => x is BaseSetting)
                        .Select(x => (BaseSetting)x)
                        .Select(x =>
                            new HurlSettingContainer(fileDocument, environmentSection, x, true, false)
                        )
            );
            fileDocument.SettingSections.Insert(sectionIdx++, environmentSection);
            sections.Add(environmentSection);

            // Collection settings
            HurlSettingSection collectionSection = new HurlSettingSection(fileDocument, Model.Enums.HurlSettingSectionType.Collection, file.Collection);
            collectionSection.SettingContainers.AddRangeIfNotNull(
                file.Collection
                    .Collection
                    .CollectionSettings
                    .Where(x => x is BaseSetting)
                    .Select(x => (BaseSetting)x)
                    .Select(x => new HurlSettingContainer(fileDocument, collectionSection, x, true, false)));
            fileDocument.SettingSections.Insert(sectionIdx++, collectionSection);
            sections.Add(collectionSection);

            // Folder settings
            List<HurlFolderContainer> collectionFolders = this.GetParentFolders(file.Folder);
            foreach (HurlFolderContainer folder in collectionFolders)
            {
                HurlSettingSection folderSection = new HurlSettingSection(fileDocument, Model.Enums.HurlSettingSectionType.Folder, folder);
                folderSection.SettingContainers.AddRangeIfNotNull(
                    folder.Folder?
                          .FolderSettings
                          .Where(x => x is BaseSetting)
                          .Select(x => (BaseSetting)x)
                          .Select(x => new HurlSettingContainer(fileDocument, folderSection, x, true, false)));

                fileDocument.SettingSections.Insert(sectionIdx++, folderSection);
                sections.Add(folderSection);
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
            this.BindSettingContainers(allSettingContainers);
        }

        /// <summary>
        /// Collect the setting containers from their sections, set enabled/collapsed state and bind events
        /// </summary>
        /// <param name="fileDocument"></param>
        /// <param name="uiState"></param>
        private void BindSettingContainers(List<HurlSettingContainer> settingContainers)
        {
            Model.UiState.UiState? uiState = _uiStateService.GetUiState(false);
            foreach (HurlSettingContainer hurlSettingContainer in settingContainers)
            {
                // Set collapse and enabled states if available from ui state
                if (uiState != null)
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
        private void On_HurlSettingSection_SettingSectionCollapsedChanged(object? sender, Model.EventArgs.SettingSectionCollapsedChangedEventArgs e)
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
                    _log.LogDebug($"Setting property changed: [{container.Document.File?.Location}]: [{e.Setting.GetType().Name}]:[{e.Setting.GetConfigurationValue()}]");
                }

                container.Document.HasChanges = true;
            }
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
                _uiStateService.SetSettingCollapsedState(container.GetId(), e.Collapsed);
            }
        }

        /// <summary>
        /// On setting key change -> reevaluate inheritances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_HurlSettingContainer_SettingKeyChanged(object? sender, Model.EventArgs.SettingEvaluationChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                await Task.Run(() => this.EvaluateSettingInheritances(container.Document));
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
                // File settings are "active" via the file settings "#" prefix
                // Other types are "active" via the uiState, since we want enabling/disabling on a file basis
                if(e.SettingSectionType != Model.Enums.HurlSettingSectionType.File)
                {
                    _uiStateService.SetSettingEnabledState(container.GetId(), e.Enabled);
                }

                await Task.Run(() =>
                {
                    this.EvaluateSettingInheritances(container.Document);
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
                if (source != null && source.Contains(e.SettingContainer))
                {
                    e.SettingContainer.Document.HasChanges = true;

                    int settingIdx = source.IndexOf(e.SettingContainer);
                    List<HurlSettingContainer> affectedSettingContainers = new List<HurlSettingContainer>();

                    if (e.MoveDirection == Model.Enums.MoveDirection.Up)
                    {
                        affectedSettingContainers.AddIfNotNull(e.SettingContainer);
                        affectedSettingContainers.AddIfNotNull(source.Get(settingIdx - 1));
                        source.MoveItemUp(e.SettingContainer);
                    }
                    else if (e.MoveDirection == Model.Enums.MoveDirection.Down)
                    {
                        affectedSettingContainers.AddIfNotNull(e.SettingContainer);
                        affectedSettingContainers.AddIfNotNull(source.Get(settingIdx + 1));
                        source.MoveItemDown(e.SettingContainer);
                    }
                    else if (e.MoveDirection == Model.Enums.MoveDirection.ToTop)
                    {
                        source.Remove(e.SettingContainer);
                        source.Insert(0, e.SettingContainer);
                        affectedSettingContainers.AddRange(source);
                    }
                    else if(e.MoveDirection == Model.Enums.MoveDirection.ToBottom)
                    {
                        source.Remove(e.SettingContainer);
                        source.Add(e.SettingContainer);
                        affectedSettingContainers.AddRange(source);
                    }

                    // Re-build ui state with new ids
                    if (uiState != null)
                    {
                        foreach(HurlSettingContainer affectedSettingContainer in affectedSettingContainers)
                        {
                            string affectedSettingId = affectedSettingContainer.GetId();

                            _uiStateService.SetSettingCollapsedState(affectedSettingId, affectedSettingContainer.Collapsed);
                        }
                    }
                }

                // Reevaluate inheritances
                await Task.Run(() => this.EvaluateSettingInheritances(container.Document));
            }
        }

        /// <summary>
        /// Reevaluates the inheritance types into 'Overwritten' flags
        /// </summary>
        /// <param name="fileDocument"></param>
        private void EvaluateSettingInheritances(FileDocumentViewModel fileDocument)
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
                    ( // For File sections the files' enabled state is important, for other types it's the containers' enabled state 
                      (container.Section.SectionType == Model.Enums.HurlSettingSectionType.File && container.Setting.IsEnabled) || 
                      (container.Section.SectionType != Model.Enums.HurlSettingSectionType.File && container.IsEnabled)
                    ) &&
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
        /// Close a file
        /// -> properly unbind all setting events
        /// </summary>
        /// <param name="fileDocument"></param>
        /// <returns>true, if the file was closed</returns>
        public async Task<bool> CloseFileDocument(FileDocumentViewModel? fileDocument)
        {
            if (fileDocument == null) return true;
            List<HurlSettingContainer> allSettingContainers = fileDocument.SettingSections.SelectMany(x => x.SettingContainers).ToList();

            foreach (HurlSettingContainer hurlSettingContainer in allSettingContainers)
            {
                hurlSettingContainer.SettingEnabledChanged -= this.On_HurlSettingContainer_SettingEnabledChanged;
                hurlSettingContainer.SettingOrderChanged -= this.On_HurlSettingContainer_SettingOrderChanged;
                hurlSettingContainer.SettingKeyChanged -= this.On_HurlSettingContainer_SettingKeyChanged;
            }

            return true;
        }

        /// <summary>
        /// Opens a folder settings document
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task OpenFolderSettings(HurlFolderContainer folder)
        {
            throw new NotImplementedException();
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
        /// Returns a list of folders in the hierarchy
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private List<HurlFolderContainer> GetParentFolders(HurlFolderContainer folder)
        {
            List<HurlFolderContainer> collectionFolders = new List<HurlFolderContainer>();

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
        private FileDocumentViewModel? GetFileDocumentByFile(HurlFileContainer file)
        {
            FileDocumentViewModel? openDocument = _editorViewViewModel.Documents.Where(x => x is FileDocumentViewModel)
                                                                                .Select(x => (FileDocumentViewModel)x)
                                                                                .Where(x => x.File != null && x.File.Location.Equals(file.Location))
                                                                                .FirstOrDefault();

            return openDocument;
        }

        /// <summary>
        /// Saves the given file
        ///  > Save the .hurl doc
        ///  > Save the file specific settings in the collection
        /// </summary>
        /// <param name="fileDocument"></param>
        /// <returns></returns>
        public async Task<bool> SaveFile(FileDocumentViewModel fileDocument)
        {
            if (fileDocument.Document == null) throw new ArgumentNullException(nameof(fileDocument.Document));
            if (fileDocument.File == null) throw new ArgumentNullException(nameof(fileDocument.File));

            // Async locked section
            return await _lock.LockAsync<bool>(async () =>
            {
                bool result = true;
                _log.LogInformation($"Attempting to save file [{fileDocument.File.Location}] in collection [{fileDocument.File.Collection.Collection.FileLocation}]");

                try
                {
                    _mainViewViewModel.StatusBarDetail = fileDocument.File.Location;
                    _mainViewViewModel.StatusBarStatus = Model.Enums.StatusBarStatus.SavingFile;

                    // Save the .hurl file
                    _log.LogDebug($"Saving .hurl file [{fileDocument.File.Location}]");
                    string fileContent = fileDocument.Document.CreateSnapshot().Text;
                    await System.IO.File.WriteAllTextAsync(fileDocument.File.Location, fileContent, Encoding.UTF8);
                    _log.LogDebug($"Saved .hurl file [{fileDocument.File.Location}]");

                    // Save the settings
                    _log.LogDebug($"Saving file settings for [{fileDocument.File.Location}] in collection [{fileDocument.File.Collection.Collection.FileLocation}]");
                    HurlCollection hurlCollection = await _collectionService.GetCollectionAsync(fileDocument.File.Collection.Collection.FileLocation);
                    HurlFile? oldCollectionFile = hurlCollection.FileSettings.FirstOrDefault(x => x.FileLocation == fileDocument.File.File.FileLocation);

                    // Create new file for saving
                    HurlFile newCollectionFile = new HurlFile(fileDocument.File.File.FileLocation)
                    {
                        FileTitle = fileDocument.File.File?.FileTitle ?? string.Empty
                    };

                    List<HurlSettingContainer> settingContainers = fileDocument.SettingSections
                                                                        .Where(x => x.SectionType == Model.Enums.HurlSettingSectionType.File)
                                                                        .SelectMany(x => x.SettingContainers).ToList();
                    foreach (HurlSettingContainer settingContainer in settingContainers)
                    {
                        newCollectionFile.FileSettings.Add(settingContainer.Setting);
                    }

                    if (oldCollectionFile != null)
                    {
                        // Replace old file
                        _log.LogDebug($"Replacing file settings for [{fileDocument.File.Location}] in collection [{fileDocument.File.Collection.Collection.FileLocation}]");
                        int oldCollectionFileIdx = hurlCollection.FileSettings.IndexOf(oldCollectionFile);
                        hurlCollection.FileSettings[oldCollectionFileIdx] = newCollectionFile;
                    }
                    else
                    {
                        // Add file
                        _log.LogDebug($"Adding file settings for [{fileDocument.File.Location}] to collection [{fileDocument.File.Collection.Collection.FileLocation}]");
                        hurlCollection.FileSettings.Add(newCollectionFile);
                    }

                    // serialize the collection
                    _log.LogDebug($"Storing collection [{hurlCollection.FileLocation}]");
                    await _collectionService.StoreCollectionAsync(hurlCollection, hurlCollection.FileLocation);
                    _log.LogDebug($"Stored collection [{hurlCollection.FileLocation}]");

                    fileDocument.HasChanges = false;
                    _mainViewViewModel.StatusBarDetail = string.Empty;
                    _mainViewViewModel.StatusBarStatus = Model.Enums.StatusBarStatus.Idle;

                    _log.LogInformation($"Saved file [{fileDocument.File.Location}] in collection [{fileDocument.File.Collection.Collection.FileLocation}]");
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

        /// <summary>
        /// Saves the currently opened file
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveCurrentFile()
        {
            if (_editorViewViewModel.DocumentDock == null) return false;
            if (_editorViewViewModel.DocumentDock.ActiveDockable == null) return false;
            if (_editorViewViewModel.DocumentDock.ActiveDockable is not FileDocumentViewModel fileDocument) return false;
            if (!fileDocument.HasChanges) return true;

            return await this.SaveFile(fileDocument);
        }

        /// <summary>
        /// Called on shutdown of the application
        /// -> saves opened docs in UI state
        /// -> closes all documents
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Shutdown()
        {
            List<string> openedFiles = new List<string>();
            List<IDockable> openDockables = new List<IDockable>();
            foreach(IDockable document in _editorViewViewModel.Documents)
            {
                if (document is FileDocumentViewModel fileDocument && fileDocument.File != null)
                {
                    openDockables.Add(fileDocument);
                    openedFiles.Add(fileDocument.File.Location);
                }
                // TODO: Folders, Collections
            }

            _uiStateService.SetOpenedFiles(openedFiles);

            // Close dockables
            foreach (IDockable dockable in openDockables)
            {
                if (!await _layoutFactory.CloseDockableAsync(dockable))
                    return false;
            }
            return true;
        }
    }
}
