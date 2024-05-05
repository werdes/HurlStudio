using ActiproSoftware.Logging;
using AvaloniaEdit.Highlighting;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Collections.Utility;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Common.Logging.Extensions;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.UiState;
using HurlStudio.Services.UiState;
using HurlStudio.Services.UserSettings;
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
        private ILogger _log = null;
        private IConfiguration _configuration = null;
        private ICollectionSerializer _collectionSerializer = null;
        private IEnvironmentSerializer _environmentSerializer = null;
        private IUserSettingsService _userSettingsService = null;
        private IUiStateService _uiStateService = null;

        private int _collectionLoaderMaxDirectoryDepth = 0;
        private string[] _collectionExplorerIgnoredDirectories = [];

        public CollectionService(ILogger<CollectionService> logger, IConfiguration configuration, ICollectionSerializer collectionSerializer, IEnvironmentSerializer environmentSerializer, IUserSettingsService userSettingsService, IUiStateService uiStateService)
        {
            _log = logger;
            _configuration = configuration;
            _collectionSerializer = collectionSerializer;
            _environmentSerializer = environmentSerializer;
            _userSettingsService = userSettingsService;
            _uiStateService = uiStateService;

            _collectionLoaderMaxDirectoryDepth = Math.Max(1, _configuration.GetValue<int>("collectionLoaderMaxDirectoryDepth"));
            _collectionExplorerIgnoredDirectories = _configuration.GetSection("collectionExplorerIgnoredDirectories").Get<string[]>() ?? [];
        }

        /// <summary>
        /// Returns a collection container for a single collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<CollectionContainer> GetCollectionContainerAsync(HurlCollection collection)
        {
            bool collapsed = true;
            Model.UiState.UiState? uiState = await _uiStateService.GetUiStateAsync(false);
            CollectionContainer collectionContainer = new CollectionContainer(collection);

            if (uiState?.ExpandedCollectionExplorerComponents?.TryGetValue(collectionContainer.GetId(), out collapsed) ?? false)
            {
                collectionContainer.Collapsed = collapsed;
            }
            string collectionRootPath = Path.GetDirectoryName(collection.FileLocation) ?? throw new ArgumentNullException(nameof(collection.FileLocation));

            // Root location
            if (!collection.ExcludeRootDirectory)
            {
                CollectionFolder? collectionRootFolder = await this.GetFolderRecursive(0, collectionContainer, null, collectionRootPath, collectionRootPath, uiState);
                if (collectionRootFolder != null)
                {
                    collectionContainer.Files.AddRange(collectionRootFolder.Files);
                    collectionContainer.Folders.AddRange(collectionRootFolder.Folders);
                }
            }

            // Additional locations
            foreach (string location in collection.AdditionalLocations)
            {
                // Provide absolute path
                string absoluteLocationPath = location.ConvertDirectorySeparator();
                if (!Path.IsPathFullyQualified(location))
                {
                    absoluteLocationPath = Path.Join(collectionRootPath, absoluteLocationPath);
                }

                if (Directory.Exists(absoluteLocationPath))
                {
                    // If either the collection root directory is excluded or the directory is not located under that root directory,
                    // go ahead and traverse the location
                    if (collection.ExcludeRootDirectory || !PathExtensions.IsChildOfDirectory(absoluteLocationPath, collectionRootPath))
                    {
                        collectionContainer.Folders.AddIfNotNull(await this.GetFolderRecursive(0, collectionContainer, null, absoluteLocationPath, collectionRootPath, uiState));
                    }
                }
                else
                {
                    collectionContainer.Folders.Add(new CollectionFolder(collectionContainer, null, absoluteLocationPath)
                    {
                        Found = false,
                        Collapsed = true
                    });
                }
            }

            return collectionContainer;
        }

        /// <summary>
        /// Provides a hierarchical structure of loaded collections
        /// </summary>
        /// <returns>List of collection containers</returns>
        public async Task<ObservableCollection<CollectionContainer>> GetCollectionContainersAsync()
        {
            ObservableCollection<CollectionContainer> collectionContainers = new ObservableCollection<CollectionContainer>();
            IEnumerable<HurlCollection> collections = await this.GetCollectionsAsync();


            // Trace Collections
            _log.LogObject(collections);

            foreach (HurlCollection collection in collections)
            {
                CollectionContainer collectionContainer = await GetCollectionContainerAsync(collection);

                collectionContainers.Add(collectionContainer);
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
        private async Task<CollectionFolder?> GetFolderRecursive(int depth, CollectionContainer collectionContainer, CollectionFolder? parent, string location, string collectionRoot, Model.UiState.UiState? uiState)
        {
            if (depth > _collectionLoaderMaxDirectoryDepth) return null;
            if (_collectionExplorerIgnoredDirectories.Contains(new DirectoryInfo(location).Name)) return null;  

            CollectionFolder collectionFolder = new CollectionFolder(collectionContainer, parent, location);
            bool folderCollapsed = false;
            if (uiState?.ExpandedCollectionExplorerComponents?.TryGetValue(collectionFolder.GetId(), out folderCollapsed) ?? false)
            {
                collectionFolder.Collapsed = folderCollapsed;
            }

            // Direct subdirectories
            List<string> folderSubdirectories = Directory.GetDirectories(location, "*", SearchOption.TopDirectoryOnly).ToList();

            // Get path of the directory relative to the location of the collection file
            string relativeDirectoryPathToCollectionRoot = Path.TrimEndingDirectorySeparator(Path.GetRelativePath(collectionRoot, location));
            collectionFolder.Folder = collectionContainer.Collection.FolderSettings.Where(x => Path.TrimEndingDirectorySeparator(x.Location.ConvertDirectorySeparator()) == relativeDirectoryPathToCollectionRoot).FirstOrDefault();

            // Recursively traverse through the directory tree
            foreach (string folderSubdirectory in folderSubdirectories)
            {
                collectionFolder.Folders.AddIfNotNull(await this.GetFolderRecursive(depth + 1, collectionContainer, collectionFolder, folderSubdirectory, collectionRoot, uiState));
            }

            // Files
            List<string> hurlFiles = Directory.GetFiles(location, $"*{GlobalConstants.HURL_FILE_EXTENSION}", SearchOption.TopDirectoryOnly).ToList();
            foreach (string hurlFile in hurlFiles)
            {
                // Get path of the file relative to the location of the collection file
                string relativeFilePathToCollectionRoot = Path.TrimEndingDirectorySeparator(Path.GetRelativePath(collectionRoot, hurlFile));
                _log.LogDebug($"Searching settings for [{hurlFile}] with relative path [{relativeDirectoryPathToCollectionRoot}]");

                // As HurlSettings are saved in the collection file under a file location,
                // try to find a setting with a matching file location
                HurlFile? fileSettings = collectionContainer.Collection.FileSettings.Where(x => Path.TrimEndingDirectorySeparator(x.FileLocation.ConvertDirectorySeparator()) == relativeFilePathToCollectionRoot).FirstOrDefault();

                CollectionFile collectionFile = new CollectionFile(collectionFolder, hurlFile);
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
                    collections.AddIfNotNull(await GetCollectionAsync(file));
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
    }
}
