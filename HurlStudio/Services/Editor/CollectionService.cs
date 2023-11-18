using ActiproSoftware.Logging;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Collections.Utility;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.CollectionContainer;
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

        private int _collectionLoaderMaxDirectoryDepth = 0;

        public CollectionService(ILogger<CollectionService> logger, IConfiguration configuration, ICollectionSerializer collectionSerializer, IEnvironmentSerializer environmentSerializer, IUserSettingsService userSettingsService)
        {
            _log = logger;
            _configuration = configuration;
            _collectionSerializer = collectionSerializer;
            _environmentSerializer = environmentSerializer;
            _userSettingsService = userSettingsService;

            _collectionLoaderMaxDirectoryDepth = Math.Max(1, _configuration.GetValue<int>("collectionLoaderMaxDirectoryDepth"));
        }

        /// <summary>
        /// Provides a hierarchical structure of loaded collections
        /// </summary>
        /// <returns>List of collection containers</returns>
        public async Task<ObservableCollection<CollectionContainer>> GetCollectionContainersAsync()
        {
            ObservableCollection<CollectionContainer> collectionContainers = new ObservableCollection<CollectionContainer>();
            IEnumerable<HurlCollection> collections = await GetCollectionsAsync();

            foreach (HurlCollection collection in collections)
            {
                CollectionContainer collectionContainer = new CollectionContainer(collection);
                string collectionRootPath = Path.GetDirectoryName(collection.FileLocation) ?? string.Empty;

                foreach (string location in collection.Locations)
                {
                    // Provide absolute path
                    string absoluteLocationPath = location.ConvertDirectorySeparator();
                    if (!Path.IsPathFullyQualified(location))
                    {
                        absoluteLocationPath = Path.Join(collectionRootPath, absoluteLocationPath);
                    }

                    collectionContainer.Folders.AddIfNotNull(await GetFolderRecursive(0, collectionContainer, absoluteLocationPath, collectionRootPath));
                }

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
        private async Task<CollectionFolder?> GetFolderRecursive(int depth, CollectionContainer collectionContainer, string location, string collectionRoot)
        {
            if (depth > _collectionLoaderMaxDirectoryDepth) return null;
            CollectionFolder collectionFolder = new CollectionFolder(collectionContainer, location);

            // Direct subdirectories
            List<string> folderSubdirectories = Directory.GetDirectories(location, "*", SearchOption.TopDirectoryOnly).ToList();

            // Get path of the directory relative to the location of the collection file
            string relativeDirectoryPathToCollectionRoot = Path.TrimEndingDirectorySeparator(Path.GetRelativePath(collectionRoot, location));
            collectionFolder.Folder = collectionContainer.Collection.FolderSettings.Where(x => Path.TrimEndingDirectorySeparator(x.Location.ConvertDirectorySeparator()) == relativeDirectoryPathToCollectionRoot).FirstOrDefault();

            // Recursively traverse through the directory tree
            foreach (string folderSubdirectory in folderSubdirectories)
            {
                collectionFolder.Folders.AddIfNotNull(await GetFolderRecursive(depth + 1, collectionContainer, folderSubdirectory, collectionRoot));
            }

            // Files
            List<string> hurlFiles = Directory.GetFiles(location, $"*{GlobalConstants.HURL_FILE_EXTENSION}", SearchOption.TopDirectoryOnly).ToList();
            foreach (string hurlFile in hurlFiles)
            {
                // Get path of the file relative to the location of the collection file
                string relativeFilePathToCollectionRoot = Path.TrimEndingDirectorySeparator(Path.GetRelativePath(collectionRoot, hurlFile));

                // As Settings are saved in the collection file under a file location,
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
                    HurlCollection collection = await _collectionSerializer.DeserializeFileAsync(file, Encoding.UTF8);
                    if (collection != null)
                    {
                        collections.Add(collection);
                    }
                }
            }

            return collections;
        }
    }
}
