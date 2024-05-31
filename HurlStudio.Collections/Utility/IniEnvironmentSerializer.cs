using HurlStudio.Collections.Model;
using HurlStudio.Collections.Model.Serializer;
using HurlStudio.Collections.Settings;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HurlStudio.Collections.Utility
{
    public class IniEnvironmentSerializer : IEnvironmentSerializer
    {
        private const string SECTION_GENERAL_HEADER = "[Environment]";
        private const string SECTION_GENERAL_NAME_KEY = "name";
        private const string SECTION_ENVIRONMENT_SETTINGS_HEADER = "[EnvironmentSettings]";
        private readonly Regex NEWLINE_REGEX = new Regex(@"\r\n?|\n", RegexOptions.Compiled);


        private IniSettingParser _settingParser;

        public IniEnvironmentSerializer(IniSettingParser settingParser)
        {
            _settingParser = settingParser;
        }

        /// <summary>
        /// Deserializes a environment string
        /// </summary>
        /// <param name="environmentContent"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public HurlEnvironment Deserialize(string environmentContent, string filePath)
        {
            environmentContent = NEWLINE_REGEX.Replace(environmentContent, Environment.NewLine);

            string[] lines = environmentContent.Split(Environment.NewLine);
            List<HurlEnvironmentSectionContainer> environmentSections = this.SplitIntoSections(lines);
            HurlEnvironment environment = new HurlEnvironment(filePath);


            foreach (HurlEnvironmentSectionContainer sectionContainer in environmentSections)
            {
                switch (sectionContainer.Type)
                {
                    case EnvironmentSectionType.General:
                        this.DeserializeGeneralSection(sectionContainer.Lines, ref environment);
                        break;
                    case EnvironmentSectionType.EnvironmentSettings:
                        this.DeserializeEnvironmentSettingsSection(sectionContainer.Lines, ref environment);
                        break;
                }
            }

            return environment;
        }

        /// <summary>
        /// Deserializes the general part of the environment
        /// </summary>
        /// <param name="lines">Plain text lines of a EnvironmentSectionType.General section container</param>
        /// <param name="environment">Target environment</param>
        private void DeserializeGeneralSection(List<string> lines, ref HurlEnvironment environment)
        {
            foreach (string line in lines)
            {
                string nameKey = $"{SECTION_GENERAL_NAME_KEY}=";
                if (line.StartsWith(nameKey))
                {
                    environment.Name = line.Split("=").Get(1)?.DecodeUrl() ?? string.Empty;
                }
            }
        }


        /// <summary>
        /// Splits the file content into sections
        /// </summary>
        /// <param name="lines">full environment file content</param>
        /// <returns>A list of containers with corresponding lines</returns>
        private List<HurlEnvironmentSectionContainer> SplitIntoSections(string[] lines)
        {
            List<HurlEnvironmentSectionContainer> sections = new List<HurlEnvironmentSectionContainer>();
            HurlEnvironmentSectionContainer currentSection = new HurlEnvironmentSectionContainer();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Create a new section on every known header, then fill in the lines
                switch (line.Trim())
                {
                    case SECTION_GENERAL_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlEnvironmentSectionContainer(EnvironmentSectionType.General);
                        break;
                    case SECTION_ENVIRONMENT_SETTINGS_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlEnvironmentSectionContainer(EnvironmentSectionType.EnvironmentSettings);
                        break;
                    default:
                        currentSection.Lines.Add(line);
                        break;
                }
            }

            // Add last section
            if (currentSection.Lines.Count > 0)
            {
                sections.Add(currentSection);
            }

            return sections.Where(x => x.Type != EnvironmentSectionType.None).ToList();
        }

        /// <summary>
        /// Deserializes the environment settings 
        /// </summary>
        /// <param name="lines">plain text lines of a EnvironmentSectionType.EnvironmentSettings section container</param>
        /// <param name="environment">Target environment</param>
        private void DeserializeEnvironmentSettingsSection(List<string> lines, ref HurlEnvironment environment)
        {
            foreach (string line in lines)
            {
                IHurlSetting? hurlSetting = _settingParser.Parse(line);
                if (hurlSetting != null)
                {
                    environment.EnvironmentSettings.Add(hurlSetting);
                }
            }
        }

        /// <summary>
        /// Deserializes a .hurle environment file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public async Task<HurlEnvironment> DeserializeFileAsync(string filePath, Encoding encoding)
        {
            string fileContent = await File.ReadAllTextAsync(filePath, encoding);
            return this.Deserialize(fileContent, filePath);
        }

        /// <summary>
        /// Serializes an environment
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public string Serialize(HurlEnvironment environment)
        {
            StringBuilder builder = new StringBuilder();

            // General section
            builder.AppendLine(SECTION_GENERAL_HEADER);
            builder.AppendLine($"{SECTION_GENERAL_NAME_KEY}={environment.Name?.EncodeUrl()}");
            builder.AppendLine();

            // Environment settings section
            builder.AppendLine(SECTION_ENVIRONMENT_SETTINGS_HEADER);
            foreach (IHurlSetting collectionSetting in environment.EnvironmentSettings)
            {
                builder.AppendLine(collectionSetting.GetConfigurationString());
            }

            return builder.ToString();
        }

        /// <summary>
        /// Serializes an enviroment as a .hurle file
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public async Task SerializeFileAsync(HurlEnvironment environment, string filePath, Encoding encoding)
        {
            string serializedContent = await Task.Run(() => this.Serialize(environment));
            await File.WriteAllTextAsync(filePath, serializedContent, encoding);
        }
    }
}
