using HurlStudio.Collections.Model;
using HurlStudio.Collections.Model.Containers;
using HurlStudio.Collections.Model.Exceptions;
using HurlStudio.Collections.Utility;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.UiState;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Localization;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public class CollectionService : ICollectionService
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private ICollectionSerializer _collectionSerializer;
        private IEnvironmentSerializer _environmentSerializer;
        private IUserSettingsService _userSettingsService;
        private IUiStateService _uiStateService;
        private INotificationService _notificationService;

        private int _collectionLoaderMaxDirectoryDepth = 0;
        private string[] _collectionExplorerIgnoredDirectories = [];
        private SemaphoreLock _lock = new SemaphoreLock();

        public CollectionService(ILogger<CollectionService> logger, IConfiguration configuration, ICollectionSerializer collectionSerializer, IEnvironmentSerializer environmentSerializer, IUserSettingsService userSettingsService, IUiStateService uiStateService, INotificationService notificationService)
        {
            _log = logger;
            _configuration = configuration;
            _collectionSerializer = collectionSerializer;
            _environmentSerializer = environmentSerializer;
            _userSettingsService = userSettingsService;
            _uiStateService = uiStateService;
            _notificationService = notificationService;

            _collectionLoaderMaxDirectoryDepth = Math.Max(1, _configuration.GetValue<int>("collectionLoaderMaxDirectoryDepth"));
            _collectionExplorerIgnoredDirectories = _configuration.GetSection("collectionExplorerIgnoredDirectories").Get<string[]>() ?? [];
        }

        /// <summary>
        /// Returns a collection container for a single collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<HurlCollectionContainer> GetCollectionContainerAsync(HurlCollection collection)
        {
            HurlCollectionContainer collectionContainer = new HurlCollectionContainer(collection);

            await this.SetCollectionContainerAsync(collectionContainer, collection);

            return collectionContainer;
        }

        /// <summary>
        /// Fills a collection container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<HurlCollectionContainer> SetCollectionContainerAsync(HurlCollectionContainer collectionContainer, HurlCollection collection)
        {
            // Reset container first
            collectionContainer.Folders.RemoveAll(x => true);
            collectionContainer.Files.RemoveAll(x => true);

            collectionContainer.Collection = collection;

            bool collapsed = true;
            Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(false);
            if (uiState?.ExpandedCollectionExplorerComponents?.TryGetValue(collectionContainer.GetId(), out collapsed) ?? false)
            {
                collectionContainer.Collapsed = collapsed;
            }
            string collectionRootPath = Path.GetDirectoryName(collection.CollectionFileLocation) ?? throw new ArgumentNullException(nameof(collection.CollectionFileLocation));

            // Root location
            if (!collection.ExcludeRootDirectory)
            {
                HurlFolderContainer? collectionRootFolder = await this.GetFolderRecursive(0, collectionContainer, null, collectionRootPath, collectionRootPath, uiState, false);
                if (collectionRootFolder != null)
                {
                    collectionContainer.Files.AddRange(collectionRootFolder.Files);
                    collectionContainer.Folders.AddRange(collectionRootFolder.Folders);
                }
            }

            // Additional locations
            foreach (AdditionalLocation location in collection.AdditionalLocations)
            {
                // Provide absolute path
                string absoluteLocationPath = location.Path.ConvertDirectorySeparator();
                if (!Path.IsPathFullyQualified(location.Path))
                {
                    absoluteLocationPath = Path.Join(collectionRootPath, absoluteLocationPath);
                }

                if (Directory.Exists(absoluteLocationPath))
                {
                    // If either the collection root directory is excluded or the directory is not located under that root directory,
                    // go ahead and traverse the location
                    if (collection.ExcludeRootDirectory || !PathExtensions.IsChildOfDirectory(absoluteLocationPath, collectionRootPath))
                    {
                        collectionContainer.Folders.AddIfNotNull(await this.GetFolderRecursive(0, collectionContainer, null, absoluteLocationPath, collectionRootPath, uiState, true));
                    }
                }
                else
                {
                    HurlFolderContainer hurlFolderContainer = new HurlFolderContainer(collectionContainer, null, new HurlFolder(location.Path), absoluteLocationPath)
                    {
                        Found = false,
                        Collapsed = true
                    };

                    collectionContainer.Folders.Add(hurlFolderContainer);
                }
            }

            return collectionContainer;
        }

        /// <summary>
        /// Provides a hierarchical structure of loaded collections
        /// </summary>
        /// <returns>List of collection containers</returns>
        public async Task<ObservableCollection<HurlCollectionContainer>> GetCollectionContainersAsync()
        {
            ObservableCollection<HurlCollectionContainer> collectionContainers = new ObservableCollection<HurlCollectionContainer>();
            IEnumerable<HurlCollection> collections = await this.GetCollectionsAsync();

            foreach (HurlCollection collection in collections)
            {
                HurlCollectionContainer collectionContainer = await this.GetCollectionContainerAsync(collection);
                collectionContainers.Add(collectionContainer);
                _log.LogInformation($"Opened collection: [{collection.CollectionFileLocation}]");
            }

            return collectionContainers;
        }

        /// <summary>
        /// Recursively expands a directory searching for .hurl files and their settings
        /// </summary>
        /// <param name="collectionContainer">The collection container containing the path</param>
        /// <param name="location">The absolute location of the directory to be traversed</param>
        /// <param name="collectionRoot">The absolute path of the collection file</param>
        /// <returns>A Folder file</returns>
        private async Task<HurlFolderContainer?> GetFolderRecursive(int depth, HurlCollectionContainer collectionContainer, HurlFolderContainer? parent, string location, string collectionRoot, Model.UiState.UiState? uiState, bool isAdditionalLocation)
        {
            if (depth > _collectionLoaderMaxDirectoryDepth) return null;
            if (_collectionExplorerIgnoredDirectories.Contains(new DirectoryInfo(location).Name)) return null;
                        
            // Direct subdirectories
            List<string> folderSubdirectories = Directory.GetDirectories(location, "*", SearchOption.TopDirectoryOnly).ToList();
            HurlFolder? folderSettings;
            string folderBasePath;

            if (!isAdditionalLocation)
            {
                // Get path of the directory relative to the location of the collection file
                string relativeDirectoryPathToCollectionRoot = Path.TrimEndingDirectorySeparator(Path.GetRelativePath(collectionRoot, location));
                folderSettings = collectionContainer.Collection.FolderSettings.Where(x => Path.TrimEndingDirectorySeparator(x.FolderLocation.ConvertDirectorySeparator()) == relativeDirectoryPathToCollectionRoot).FirstOrDefault();
                if (folderSettings == null)
                {
                    folderSettings = new HurlFolder(relativeDirectoryPathToCollectionRoot);
                }
                folderBasePath = relativeDirectoryPathToCollectionRoot;
            }
            else
            {
                // For additional locations, take the absolute path
                string absoluteDirectoryPath = location;
                folderSettings = collectionContainer.Collection.FolderSettings.Where(x => Path.TrimEndingDirectorySeparator(x.FolderLocation.ConvertDirectorySeparator()) == absoluteDirectoryPath).FirstOrDefault();
                if (folderSettings == null)
                {
                    folderSettings = new HurlFolder(absoluteDirectoryPath);
                }
                folderBasePath = absoluteDirectoryPath;
            }

            HurlFolderContainer collectionFolder = new HurlFolderContainer(collectionContainer, parent, folderSettings, location);

            // Set ui state
            bool folderCollapsed = false;
            if (uiState?.ExpandedCollectionExplorerComponents?.TryGetValue(collectionFolder.GetId(), out folderCollapsed) ?? false)
            {
                collectionFolder.Collapsed = folderCollapsed;
            }


            // Recursively traverse through the directory tree
            foreach (string folderSubdirectory in folderSubdirectories)
            {
                collectionFolder.Folders.AddIfNotNull(await this.GetFolderRecursive(depth + 1, collectionContainer, collectionFolder, folderSubdirectory, collectionRoot, uiState, isAdditionalLocation));
            }

            // Files
            List<string> hurlFiles = Directory.GetFiles(location, $"*{GlobalConstants.HURL_FILE_EXTENSION}", SearchOption.TopDirectoryOnly).ToList();
            foreach (string hurlFile in hurlFiles)
            {
                HurlFile? fileSettings;
                if (!isAdditionalLocation)
                {
                    // Get path of the file relative to the location of the collection file
                    string relativeFilePathToCollectionRoot = Path.TrimEndingDirectorySeparator(Path.GetRelativePath(collectionRoot, hurlFile));

                    // As HurlSettings are saved in the collection file under a file location,
                    // try to find a setting with a matching file location
                    // -> If no setting block is found, create an empty one
                    fileSettings = collectionContainer.Collection.FileSettings.Where(x => Path.TrimEndingDirectorySeparator(x.FileLocation.ConvertDirectorySeparator()) == relativeFilePathToCollectionRoot).FirstOrDefault();
                    if (fileSettings == null)
                    {
                        fileSettings = new HurlFile(relativeFilePathToCollectionRoot);
                    }
                }
                else
                {
                    // Get absolute path for files in "additional paths" folders
                    string absoluteFilePath = hurlFile.ConvertDirectorySeparator();

                    // As HurlSettings are saved in the collection file under a file location,
                    // try to find a setting with a matching file location
                    // -> If no setting block is found, create an empty one
                    fileSettings = collectionContainer.Collection.FileSettings.Where(x => x.FileLocation.ConvertDirectorySeparator() == absoluteFilePath.ConvertDirectorySeparator()).FirstOrDefault();
                    if (fileSettings == null)
                    {
                        fileSettings = new HurlFile(absoluteFilePath);
                    }
                }

                HurlFileContainer collectionFile = new HurlFileContainer(collectionFolder, fileSettings, hurlFile);
                collectionFile.File = fileSettings;

                collectionFolder.Files.Add(collectionFile);
            }

            return collectionFolder;
        }

        /// <summary>
        /// Deserializes collections from user settings
        /// </summary>
        /// <returns>A list of collections, configured in user settings</returns>
        public async Task<IEnumerable<HurlCollection>> GetCollectionsAsync()
        {
            List<HurlCollection> collections = new List<HurlCollection>();
            Model.UserSettings.UserSettings? userSettings = await _userSettingsService.GetUserSettingsAsync(true);
            if (userSettings != null && userSettings.CollectionFiles != null)
            {
                IEnumerable<string> files = userSettings.CollectionFiles;
                foreach (string file in files)
                {
                    try
                    {
                        collections.AddIfNotNull(await this.GetCollectionAsync(file));
                    }
                    catch (SettingParserException ex)
                    {
                        _log.LogException(ex);
                        _notificationService.Notify(
                            Model.Notifications.NotificationType.Error,
                            Localization.Service_CollectionService_Errors_SettingParserError_Title,
                            $"{Localization.Service_CollectionService_Errors_SettingParserError_Text}: Collection {file}: {ex.Message}");
                    }
                }
            }

            return collections;
        }

        /// <summary>
        /// Deserializes a single collection
        /// </summary>
        /// <param name="collectionLocation"></param>
        /// <returns></returns>
        public async Task<HurlCollection> GetCollectionAsync(string collectionLocation)
        {
            return await _collectionSerializer.DeserializeFileAsync(collectionLocation, Encoding.UTF8);
        }

        /// <summary>
        /// Serializes a collection and stores it at the given location
        /// </summary>
        /// <param name="collection">the collection to be stored</param>
        /// <param name="collectionLocation">the location</param>
        /// <returns></returns>
        public async Task StoreCollectionAsync(HurlCollection collection, string collectionLocation)
        {
            _log.LogDebug($"Storing collection [{collection.Name}] to [{collectionLocation}]");

            await _lock.LockAsync(async () =>
            {
                await _collectionSerializer.SerializeFileAsync(collection, collectionLocation, Encoding.UTF8);
            });
        }
    }
}
