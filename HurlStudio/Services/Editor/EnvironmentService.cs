using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Collections.Utility;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Services.UserSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public class EnvironmentService : IEnvironmentService
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IEnvironmentSerializer _environmentSerializer;
        private IUserSettingsService _userSettingsService;

        public EnvironmentService(ILogger<CollectionService> logger, IConfiguration configuration, IEnvironmentSerializer environmentSerializer, IUserSettingsService userSettingsService)
        {
            _log = logger;
            _configuration = configuration;
            _environmentSerializer = environmentSerializer;
            _userSettingsService = userSettingsService;
        }

        /// <summary>
        /// Deserializes the environment at the given location
        /// </summary>
        /// <param name="enviromentLocation"></param>
        /// <returns></returns>
        public async Task<HurlEnvironment> GetEnvironmentAsync(string enviromentLocation)
        {
            return await _environmentSerializer.DeserializeFileAsync(enviromentLocation, Encoding.UTF8);
        }

        public async Task<HurlEnvironmentContainer> GetEnvironmentContainerAsync(HurlEnvironment enviroment)
        {
            return new HurlEnvironmentContainer(enviroment, enviroment.FileLocation);
        }

        /// <summary>
        /// Returns a list of available environment containers
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<HurlEnvironmentContainer>> GetEnvironmentContainersAsync()
        {
            ObservableCollection<HurlEnvironmentContainer> environmentContainers = new ObservableCollection<HurlEnvironmentContainer>();
            IEnumerable<HurlEnvironment> environments = await this.GetEnvironmentsAsync();

            // Trace Collections
            _log.LogObject(environments);

            foreach (HurlEnvironment environment in environments)
            {
                HurlEnvironmentContainer environmentContainer = await GetEnvironmentContainerAsync(environment);
                environmentContainers.Add(environmentContainer);
            }

            return environmentContainers;
        }

        /// <summary>
        /// Deserializes environments from  user settings
        /// </summary>
        /// <returns>A list of environments, configured in user settings</returns>
        public async Task<IEnumerable<HurlEnvironment>> GetEnvironmentsAsync()
        {
            List<HurlEnvironment> enviroments = new List<HurlEnvironment>();
            Model.UserSettings.UserSettings? userSettings = await _userSettingsService.GetUserSettingsAsync(true);
            string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GlobalConstants.APPLICATION_DIRECTORY_NAME);
            string[] environmentFiles = Directory.GetFiles(Path.Combine(baseDir, GlobalConstants.ENVIRONMENTS_DIRECTORY_NAME), $"*{GlobalConstants.ENVIRONMENT_FILE_EXTENSION}");

            if(!environmentFiles.Any())
            {
                environmentFiles = new string[] { await this.BuildDefaultEnvironment() };
            }

            foreach (string environmentFile in environmentFiles)
            {
                HurlEnvironment hurlEnvironment = await this.GetEnvironmentAsync(environmentFile);
                enviroments.Add(hurlEnvironment);
            }

            return enviroments;
        }

        /// <summary>
        /// Creates a default environment file
        /// </summary>
        /// <returns></returns>
        private async Task<string> BuildDefaultEnvironment()
        {
            HurlEnvironment hurlEnvironment = this.GetDefaultEnvironment();
            await _environmentSerializer.SerializeFileAsync(hurlEnvironment, hurlEnvironment.FileLocation, Encoding.UTF8);
            return hurlEnvironment.FileLocation;
        }

        /// <summary>
        /// Gets an empty default environment
        /// </summary>
        /// <returns></returns>
        private HurlEnvironment GetDefaultEnvironment()
        {
            string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GlobalConstants.APPLICATION_DIRECTORY_NAME);
            string fileName = _configuration.GetValue<string>("defaultValues:environmentName")?.Normalize() + GlobalConstants.ENVIRONMENT_FILE_EXTENSION;
            string environmentFile = Path.Combine(baseDir, GlobalConstants.ENVIRONMENTS_DIRECTORY_NAME, fileName);

            return new HurlEnvironment(environmentFile)
            {
                Name = _configuration.GetValue<string>("defaultValues:environmentName")
            };
        }
    }
}
