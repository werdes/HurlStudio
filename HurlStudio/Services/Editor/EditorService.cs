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
        private IConfiguration _configuration;

        private int _fileHistoryLength = 0;

        public EditorService(ILogger<EditorService> logger, ServiceManager<Document> documentControlBuilder, EditorViewViewModel editorViewViewModel, MainViewViewModel mainViewViewModel, LayoutFactory layoutFactory, IUserSettingsService userSettingsService, INotificationService notificationService, IConfiguration configuration)
        {
            _log = logger;
            _documentControlBuilder = documentControlBuilder;
            _editorViewViewModel = editorViewViewModel;
            _mainViewViewModel = mainViewViewModel;
            _layoutFactory = layoutFactory;
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
        /// Opens a collection file as a document 
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

                IDockable? openDocument = _editorViewViewModel.Documents.Where(x => x is FileDocumentViewModel)
                                                                        .Select(x => (FileDocumentViewModel)x)
                                                                        .Where(x => x.File != null && x.File.Equals(file))
                                                                        .FirstOrDefault();

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


                    // Place file in history
                    _editorViewViewModel.FileHistoryEntries.RemoveAll(x => x.Location == fileDocument.File?.Location);
                    _editorViewViewModel.FileHistoryEntries.Add(new Model.UiState.FileHistoryEntry(file.Location, DateTime.UtcNow));
                    _editorViewViewModel.FileHistoryEntries = new ObservableCollection<Model.UiState.FileHistoryEntry>(_editorViewViewModel.FileHistoryEntries.OrderByDescending(x => x.LastOpened).Take(_fileHistoryLength));
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
        /// Consolidates a list of settings for a given file into the document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileDocument"></param>
        private void LoadSettingsForFile(CollectionFile file, FileDocumentViewModel fileDocument)
        {
            fileDocument.EnvironmentSettingContainers.Clear();
            fileDocument.CollectionSettingContainers.Clear();
            fileDocument.FolderSettingContainers.Clear();
            fileDocument.FileSettingContainers.Clear();

            // Environment settings
            fileDocument.EnvironmentSettingContainers.AddRangeIfNotNull(
                                _editorViewViewModel.ActiveEnvironment?.CollectionSettings.Where(x => x is BaseSetting)
                                                                                          .Select(x => (BaseSetting)x)
                                                                                          .Select(x => new HurlSettingContainer(fileDocument, x, true)));

            // Collection settings
            fileDocument.CollectionSettingContainers.AddRangeIfNotNull(
                file.Collection.Collection.CollectionSettings.Where(x => x is BaseSetting)
                                                             .Select(x => (BaseSetting)x)
                                                             .Select(x => new HurlSettingContainer(fileDocument, x, true)));

            // Folder settings
            List<CollectionFolder> collectionFolders = this.GetParentFolders(file.Folder);
            foreach (CollectionFolder folder in collectionFolders)
            {
                FolderSettingContainer folderSettingContainer = new FolderSettingContainer(folder);
                folderSettingContainer.Containers.AddRangeIfNotNull(folder.Folder?.FolderSettings.Where(x => x is BaseSetting)
                                                                                  .Select(x => (BaseSetting)x)
                                                                                  .Select(x => new HurlSettingContainer(fileDocument, x, true)));

                fileDocument.FolderSettingContainers.Add(folderSettingContainer);
            }

            // File settings
            fileDocument.FileSettingContainers.AddRangeIfNotNull(
                file.File?.FileSettings.Where(x => x is BaseSetting)
                                       .Select(x => (BaseSetting)x)
                                       .Select(x => new HurlSettingContainer(fileDocument, x, false)));


            // Inheritance behavior
            List<HurlSettingContainer> allSettingContainers = new List<HurlSettingContainer>();
            allSettingContainers.AddRange(fileDocument.FileSettingContainers);
            allSettingContainers.AddRange(fileDocument.FolderSettingContainers.SelectMany(x => x.Containers).Reverse());
            allSettingContainers.AddRange(fileDocument.CollectionSettingContainers);
            allSettingContainers.AddRange(fileDocument.EnvironmentSettingContainers);

            foreach (HurlSettingContainer hurlSettingContainer in allSettingContainers)
            {
                hurlSettingContainer.SettingEnabledChanged += On_HurlSettingContainer_SettingEnabledChanged;
                hurlSettingContainer.SettingOrderChanged += On_HurlSettingContainer_SettingOrderChanged;
                hurlSettingContainer.SettingKeyChanged += On_HurlSettingContainer_SettingKeyChanged;
            }

            this.EvaluateInheritances(fileDocument, true);
        }

        /// <summary>
        /// On Setting key change -> reevaluate inheritances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void On_HurlSettingContainer_SettingKeyChanged(object? sender, Model.EventArgs.SettingKeyChangedEventArgs e)
        {
            if (sender != null && sender is HurlSettingContainer container)
            {
                await Task.Run(() => this.EvaluateInheritances(container.Document, false));
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
                OrderedObservableCollection<HurlSettingContainer>? source = null;

                // Get Source
                if (container.Document.EnvironmentSettingContainers.Contains(e.Setting))
                {
                    source = container.Document.EnvironmentSettingContainers;
                }
                if (container.Document.CollectionSettingContainers.Contains(e.Setting))
                {
                    source = container.Document.CollectionSettingContainers;
                }
                if (container.Document.FileSettingContainers.Contains(e.Setting))
                {
                    source = container.Document.FileSettingContainers;
                }
                foreach (FolderSettingContainer folderSettingContainer in container.Document.FolderSettingContainers)
                {
                    if (folderSettingContainer.Containers.Contains(e.Setting))
                    {
                        source = folderSettingContainer.Containers;
                    }
                }

                if(source != null)
                {
                    switch (e.MoveDirection)
                    {
                        case Model.Enums.MoveDirection.Up:
                            source.MoveItemUp(e.Setting);
                            break;
                        case Model.Enums.MoveDirection.Down:
                            source.MoveItemDown(e.Setting);
                            break;
                    }

                    // Reevaluate inheritances
                    await Task.Run(() => this.EvaluateInheritances(container.Document, false));
                }
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
                await Task.Run(() => this.EvaluateInheritances(container.Document, false));
            }
        }

        /// <summary>
        /// Reevaluates the inheritance types into 'Overwritten' flags
        /// </summary>
        /// <param name="fileDocument"></param>
        private void EvaluateInheritances(FileDocumentViewModel fileDocument, bool collapseContainers)
        {
            List<HurlSettingContainer> allSettingContainers = new List<HurlSettingContainer>();
            allSettingContainers.AddRange(fileDocument.EnvironmentSettingContainers);
            allSettingContainers.AddRange(fileDocument.CollectionSettingContainers);
            allSettingContainers.AddRange(fileDocument.FolderSettingContainers.SelectMany(x => x.Containers));
            allSettingContainers.AddRange(fileDocument.FileSettingContainers);

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
                            allSettingContainers[j].Collapsed = collapseContainers ? true : allSettingContainers[j].Collapsed;
                        }
                        // Same type on unique key setting -> compare configuration key, if matching set container to overwritten
                        else if (container.Setting.GetInheritanceBehavior() == Common.Enums.HurlSettingInheritanceBehavior.UniqueKey &&
                                allSettingContainers[j].Setting.GetType() == container.Setting.GetType() &&
                                allSettingContainers[j].Setting.GetConfigurationKey() == container.Setting.GetConfigurationKey())
                        {
                            allSettingContainers[j].Overwritten = true;
                            allSettingContainers[j].Collapsed = collapseContainers ? true : allSettingContainers[j].Collapsed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Opens a file from just its path
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
    }
}
