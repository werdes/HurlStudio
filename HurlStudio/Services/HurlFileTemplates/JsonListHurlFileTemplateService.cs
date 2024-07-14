using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.Templates;
using HurlStudio.Collections.Settings;
using HurlStudio.Collections.Utility;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using HurlStudio.UI.Localization;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HurlStudio.Services.HurlFileTemplates
{
    public class JsonListHurlFileTemplateService : IHurlFileTemplateService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly SemaphoreLock _semaphoreLock;
        private List<HurlFileTemplate>? _templates;
        private ISettingParser _settingParser;
        private JsonSerializerOptions _serializerOptions;

        public JsonListHurlFileTemplateService(ILogger<JsonListHurlFileTemplateService> logger, IConfiguration configuration, IniSettingParser settingParser)
        {
            _logger = logger;
            _configuration = configuration;
            _settingParser = settingParser;

            _semaphoreLock = new SemaphoreLock();
            _serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Converters = {
                    new JsonStringEnumConverter()
                }
            };
        }

        /// <summary>
        /// Adds all not yet present default templates from app settings to the template list
        /// </summary>
        /// <returns></returns>
        private void AddNewDefaultTemplates()
        {
            if (_templates == null) return;

            List<HurlFileTemplate> templates = _configuration.GetSection($"defaultTemplates").Get<List<HurlFileTemplate>>() ?? new List<HurlFileTemplate>();
            foreach (HurlFileTemplate template in templates)
            {
                if (_templates.Any(x => x.Id == template.Id)) continue; // Template already exists
                if (template.Name == null) continue;

                template.Name = Localization.ResourceManager.GetString(template.Name) ?? template.Name;
                template.IsDefaultTemplate = true;
                _templates.Add(template);
            }
        }

        /// <inheritdoc />
        public async Task<bool> CreateTemplateAsync(HurlFileTemplate template)
        {
            if (_templates == null) await this.LoadTemplatesAsync();
            if (_templates == null) return false;

            if (_templates.Any(x => x.Id == template.Id)) return false;
            _templates.Add(template);

            return await this.StoreTemplatesAsync();
        }

        /// <inheritdoc />
        public async Task<bool> DeleteTemplateAsync(Guid id)
        {
            if (_templates == null) await this.LoadTemplatesAsync();
            if (_templates == null) return false;

            HurlFileTemplate? template = await this.GetHurlFileTemplateByIdAsync(id);
            if (template == null) return false;

            if (template.IsDefaultTemplate.HasValue && template.IsDefaultTemplate.Value)
            {
                template.IsDeleted = true;
            }
            else
            {
                _templates.Remove(template);
            }

            return await this.StoreTemplatesAsync();
        }

        /// <inheritdoc />
        public async Task<HurlFileTemplate?> GetHurlFileTemplateByIdAsync(Guid id)
        {
            if (_templates == null) await this.LoadTemplatesAsync();
            if (_templates == null) return null;

            return _templates.FirstOrDefault(x => x.Id == id);
        }

        /// <inheritdoc/>
        public async Task<List<HurlFileTemplateContainer>?> GetTemplateContainersAsync()
        {
            if (_templates == null) await this.LoadTemplatesAsync();
            if (_templates == null) return null;

            List<HurlFileTemplateContainer> containers = new List<HurlFileTemplateContainer>();
            foreach (HurlFileTemplate template in _templates)
            {
                if (template.IsDeleted.HasValue && template.IsDeleted.Value) continue; // Skip "deleted" templates
                
                HurlFileTemplateContainer? container = await this.GetTemplateContainerAsync(template.Id);
                if(container == null) continue; 

                containers.Add(container);
            }

            return containers;
        }


        /// <inheritdoc />
        public async Task<HurlFileTemplateContainer?> GetTemplateContainerAsync(Guid id)
        {
            if (_templates == null) await this.LoadTemplatesAsync();
            if (_templates == null) return null;

            HurlFileTemplate? template = _templates?.FirstOrDefault(x => x.Id == id);
            if (template == null) return null;

            HurlSettingSection settingSection = new HurlSettingSection(null, Model.Enums.HurlSettingSectionType.File, null);
            HurlFileTemplateContainer container = new HurlFileTemplateContainer(template, settingSection);

            foreach (string settingString in template.Settings)
            {
                IHurlSetting? hurlSetting = _settingParser.Parse(settingString);
                if (hurlSetting == null || hurlSetting is not BaseSetting baseSetting) continue;

                HurlSettingContainer hurlSettingContainer = new HurlSettingContainer(null, settingSection, baseSetting, true, false, Model.Enums.EnableType.Setting);
                settingSection.SettingContainers.Add(hurlSettingContainer);
            }

            return container;
        }

        /// <inheritdoc />
        public async Task<List<HurlFileTemplate>> GetTemplatesAsync(bool reload)
        {
            if (_templates == null || reload) await this.LoadTemplatesAsync();

            return _templates ?? new List<HurlFileTemplate>();
        }

        /// <inheritdoc />
        public async Task<bool> LoadTemplatesAsync()
        {
            string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                GlobalConstants.APPLICATION_DIRECTORY_NAME);
            string filePath = Path.Combine(baseDir, GlobalConstants.FILE_TEMPLATE_FILE_NAME);

            if (!File.Exists(filePath))
            {
                _templates = new List<HurlFileTemplate>();
                this.AddNewDefaultTemplates();
                await this.StoreTemplatesAsync();
            }
            else
            {
                string json = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
                _templates = JsonSerializer.Deserialize<List<HurlFileTemplate>>(json);
            }

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> StoreTemplatesAsync()
        {
            return await _semaphoreLock.LockAsync<bool>(async () =>
            {
                string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    GlobalConstants.APPLICATION_DIRECTORY_NAME);
                string filePath = Path.Combine(baseDir, GlobalConstants.FILE_TEMPLATE_FILE_NAME);

                string json = JsonSerializer.Serialize(_templates, _serializerOptions);
                await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);

                return true;
            });
        }

        /// <inheritdoc />
        public async Task<bool> UpdateTemplateAsync(HurlFileTemplate template)
        {
            if (_templates == null) await this.LoadTemplatesAsync();
            if (_templates == null) return false;

            if (!_templates.Any(x => x.Id == template.Id)) return false;
            int idx = _templates.IndexOf(x => x.Id == template.Id);

            if(idx == -1) return false;
            _templates[idx] = template;
            await this.StoreTemplatesAsync();
            await this.LoadTemplatesAsync();

            return true;
        }
    }
}