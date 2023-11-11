using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Collections.Utility;
using HurlStudio.Services.UserSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public class EnvironmentService : IEnvironmentService
    {
        private ILogger _log = null;
        private IConfiguration _configuration = null;
        private IEnvironmentSerializer _environmentSerializer = null;
        private IUserSettingsService _userSettingsService = null;

        public EnvironmentService(ILogger<CollectionService> logger, IConfiguration configuration, IEnvironmentSerializer environmentSerializer, IUserSettingsService userSettingsService)
        {
            this._log = logger;
            this._configuration = configuration;
            this._environmentSerializer = environmentSerializer;
            this._userSettingsService = userSettingsService;
        }

        /// <summary>
        /// Deserializes environments from  user settings
        /// </summary>
        /// <returns>A list of environments, configured in user settings</returns>
        public async Task<IEnumerable<HurlEnvironment>> GetEnvironmentsAsync()
        {
            List<HurlEnvironment> enviroments = new List<HurlEnvironment>();
            Model.UserSettings.UserSettings? userSettings = await _userSettingsService.GetUserSettingsAsync(true);

            //if (userSettings != null && userSettings.CollectionFiles != null)
            //{
            //    IEnumerable<string> files = userSettings.CollectionFiles;
            //    foreach (string file in files)
            //    {
            //        HurlEnvironment environment = await _environmentSerializer.DeserializeFileAsync(file, Encoding.UTF8);
            //        if (environment != null)
            //        {
            //            enviroments.Add(environment);
            //        }
            //    }
            //}

            return enviroments;
        }
    }
}
