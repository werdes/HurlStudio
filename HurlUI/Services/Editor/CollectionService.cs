using ActiproSoftware.Logging;
using HurlUI.Collections.Model.Collection;
using HurlUI.Collections.Model.Environment;
using HurlUI.Collections.Utility;
using HurlUI.Services.UserSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Services.Editor
{
    public class CollectionService : ICollectionService
    {
        private ILogger _log = null;
        private IConfiguration _configuration = null;
        private ICollectionSerializer _collectionSerializer = null;
        private IEnvironmentSerializer _environmentSerializer = null;
        private IUserSettingsService _userSettingsService = null;

        public CollectionService(ILogger<CollectionService> logger, IConfiguration configuration, ICollectionSerializer collectionSerializer, IEnvironmentSerializer environmentSerializer, IUserSettingsService userSettingsService)
        {
            this._log = logger;
            this._configuration = configuration;
            this._collectionSerializer = collectionSerializer;
            this._environmentSerializer = environmentSerializer;
            this._userSettingsService = userSettingsService;
        }

        /// <summary>
        /// Deserializes collections from  user settings
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
                    if(collection != null)
                    {
                        collections.Add(collection);
                    }
                }
            }

            return collections;
        }
    }
}
