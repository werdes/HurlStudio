using Avalonia.Styling;
using HurlStudio.Common;
using HurlStudio.Model.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HurlStudio.Services.UserSettings
{
    public class JsonUserSettingsService : IUserSettingsService
    {
        private JsonSerializerOptions _serializerOptions;
        private IConfiguration _configuration;
        private ILogger _logger;
        private Model.UserSettings.UserSettings? _userSettings;

        public JsonUserSettingsService(IConfiguration configuration, ILogger<JsonUserSettingsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _userSettings = null;
            _serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Converters = {
                    new JsonStringEnumConverter()
                }
            };
        }

        /// <summary>
        /// returns the user settings
        /// </summary>
        /// <param name="refresh">reload the settings from disk</param>
        /// <returns>The deserialized UserSettings object</returns>
        public async Task<Model.UserSettings.UserSettings> GetUserSettingsAsync(bool refresh)
        {
            if (refresh || _userSettings == null)
            {
                await this.LoadUserSettingsAsync();
            }

            return _userSettings;
        }

        /// <summary>
        /// returns the user settings
        /// </summary>
        /// <param name="refresh">reload the settings from disk</param>
        /// <returns>The deserialized UserSettings object</returns>
        public Model.UserSettings.UserSettings GetUserSettings(bool refresh)
        {
            if (refresh || _userSettings == null)
            {
                this.LoadUserSettings();
            }

            return _userSettings;
        }

        /// <summary>
        /// Stores the user settings to a .json-file on disk
        /// </summary>
        public async Task StoreUserSettingsAsync()
        {
            string path = this.GetUserSettingsFilePath();

            if (_userSettings == null) throw new ArgumentNullException($"no user setting were provided to {nameof(JsonUserSettingsService)}");

            string json = JsonSerializer.Serialize(_userSettings, _serializerOptions);
            await File.WriteAllTextAsync(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Stores the user settings to a .json-file on disk
        /// </summary>
        public void StoreUserSettings()
        {
            string path = this.GetUserSettingsFilePath();

            if (_userSettings == null) throw new ArgumentNullException($"no user setting were provided to {nameof(JsonUserSettingsService)}");

            string json = JsonSerializer.Serialize(_userSettings, _serializerOptions);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Loads the user settings from disk
        /// </summary>
        /// <returns></returns>
        private async Task LoadUserSettingsAsync()
        {
            string path = this.GetUserSettingsFilePath();
            if (File.Exists(path))
            {
                string json = await File.ReadAllTextAsync(path, Encoding.UTF8);
                _userSettings = JsonSerializer.Deserialize<Model.UserSettings.UserSettings>(json, _serializerOptions);
            }
            else
            {
                _userSettings = this.GetDefaultUserSettings();
                await this.StoreUserSettingsAsync();
            }
        }

        /// <summary>
        /// Loads the user settings from disk
        /// </summary>
        /// <returns></returns>
        private void LoadUserSettings()
        {
            string path = this.GetUserSettingsFilePath();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                _userSettings = JsonSerializer.Deserialize<Model.UserSettings.UserSettings>(json, _serializerOptions);
            }
            else
            {
                _userSettings = this.GetDefaultUserSettings();
                this.StoreUserSettings();
            }
        }

        /// <summary>
        /// Returns the path of the settings file
        /// </summary>
        /// <returns>path of the settings file</returns>
        private string GetUserSettingsFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                GlobalConstants.APPLICATION_DIRECTORY_NAME,
                                GlobalConstants.USERSETTINGS_JSON_FILE_NAME);
        }

        /// <summary>
        /// Returns a default user settings in case no file exists
        /// </summary>
        /// <returns> default user settings object</returns>
        private Model.UserSettings.UserSettings GetDefaultUserSettings()
        {
            return new Model.UserSettings.UserSettings(
                _configuration.GetValue<bool>("showInvariantCulture")
                    ? CultureInfo.InvariantCulture
                    : CultureInfo.GetCultureInfo(_configuration.GetValue<string>("defaultValues:uiLanguage") ?? "en"),
                _configuration.GetValue<ApplicationTheme>("defaultValues:theme")
            );
        }
    }
}
