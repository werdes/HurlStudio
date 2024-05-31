using HurlStudio.Collections.Model;
using HurlStudio.Collections.Model.Containers;
using HurlStudio.Collections.Model.Serializer;
using HurlStudio.Collections.Settings;
using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;
using NLog;
using System.Text;
using System.Text.RegularExpressions;

namespace HurlStudio.Collections.Utility
{
    public class IniCollectionSerializer : ICollectionSerializer
    {
        private readonly Regex NEWLINE_REGEX = new Regex(@"\r\n?|\n", RegexOptions.Compiled);
        private const string SECTION_GENERAL_HEADER = "[Collection]";
        private const string SECTION_GENERAL_NAME_KEY = "name";
        private const string SECTION_GENERAL_EXCLUDE_ROOT_KEY = "exclude_root_dir";
        private const string SECTION_ADDITIONALLOCATIONS_HEADER = "[AdditionalLocations]";
        private const string SECTION_COLLECTION_SETTINGS_HEADER = "[CollectionSettings]";
        private const string SECTION_FILE_SETTINGS_HEADER = "[FileSettings]";
        private const string SECTION_FILE_SETTINGS_LOCATION_KEY = "location";
        private const string SECTION_FILE_SETTINGS_TITLE_KEY = "title";
        private const string SECTION_FOLDER_SETTINGS_HEADER = "[FolderSettings]";
        private const string SECTION_FOLDER_SETTINGS_LOCATION_KEY = "location";


        private IniSettingParser _settingParser;

        public IniCollectionSerializer(IniSettingParser settingParser)
        {
            _settingParser = settingParser;
        }

        /// <summary>
        /// Deserializes a .hurlc formatted string
        /// </summary>
        /// <param name="collectionContent">.hurlc file content</param>
        /// <returns></returns>
        public HurlCollection Deserialize(string collectionContent, string path)
        {
            collectionContent = NEWLINE_REGEX.Replace(collectionContent, Environment.NewLine);

            string[] lines = collectionContent.Split(Environment.NewLine);
            List<HurlCollectionSectionContainer> collectionSections = this.SplitIntoSections(lines);
            HurlCollection collection = new HurlCollection(path);

            foreach (HurlCollectionSectionContainer sectionContainer in collectionSections)
            {
                switch (sectionContainer.Type)
                {
                    case CollectionSectionType.General:
                        this.DeserializeGeneralSection(sectionContainer.Lines, collection);
                        break;
                    case CollectionSectionType.AdditionalLocations:
                        this.DeserializeAdditionalLocationsSection(sectionContainer.Lines, collection);
                        break;
                    case CollectionSectionType.CollectionSettings:
                        this.DeserializeCollectionSettingsSection(sectionContainer.Lines, collection);
                        break;
                    case CollectionSectionType.FileSettings:
                        this.DeserializeFileSettingsSection(sectionContainer.Lines, collection);
                        break;
                    case CollectionSectionType.FolderSettings:
                        this.DeserializeFolderSettingsSection(sectionContainer.Lines, collection);
                        break;
                }
            }


            return collection;
        }

        /// <summary>
        /// Deserializes the folder settings part of the collection
        /// </summary>
        /// <param name="lines">plain text lines of a CollectionSectionType.FolderSettings section container</param>
        /// <param name="collection">Target collection</param>  
        private void DeserializeFolderSettingsSection(List<string> lines, HurlCollection collection)
        {
            HurlFolder hurlFolder = new HurlFolder();

            foreach (string line in lines)
            {
                string locationKey = SECTION_FOLDER_SETTINGS_LOCATION_KEY + "=";

                if (line.StartsWith(locationKey))
                {
                    // Folder path
                    hurlFolder.FolderLocation = line.Split('=').Get(1) ?? string.Empty;
                }
                else
                {
                    // Serializable setting
                    IHurlSetting? hurlSetting = _settingParser.Parse(line);
                    if (hurlSetting != null)
                    {
                        hurlFolder.FolderSettings.Add(hurlSetting);
                    }
                }
            }

            collection.FolderSettings.Add(hurlFolder);
        }

        /// <summary>
        /// Deserializes the file settings part of the collection
        /// </summary>
        /// <param name="lines">plain text lines of a CollectionSectionType.FileSettings section container</param>
        /// <param name="collection">Target collection</param>
        /// <exception cref="ArgumentNullException">if no location is provided</exception>
        private void DeserializeFileSettingsSection(List<string> lines, HurlCollection collection)
        {
            HurlFile hurlFile = new HurlFile();

            foreach (string line in lines)
            {
                string locationKey = SECTION_FILE_SETTINGS_LOCATION_KEY + "=";
                string titleKey = SECTION_FILE_SETTINGS_TITLE_KEY + "=";

                if (line.StartsWith(locationKey))
                {
                    // .hurl file location
                    hurlFile.FileLocation = line.Split('=').Get(1) ?? string.Empty;
                }
                else if (line.StartsWith(titleKey))
                {
                    hurlFile.FileTitle = line.Split('=').Get(1) ?? string.Empty;
                }
                else
                {
                    // Serializable setting
                    IHurlSetting? hurlSetting = _settingParser.Parse(line);
                    if (hurlSetting != null)
                    {
                        hurlFile.FileSettings.Add(hurlSetting);
                    }
                }
            }

            if (string.IsNullOrEmpty(hurlFile.FileLocation)) throw new ArgumentNullException(nameof(hurlFile.FileLocation));
            collection.FileSettings.Add(hurlFile);
        }


        /// <summary>
        /// Deserializes the collection settings part of the collection
        /// </summary>
        /// <param name="lines">plain text lines of a CollectionSectionType.Settings section container</param>
        /// <param name="collection">Target collection</param>
        private void DeserializeCollectionSettingsSection(List<string> lines, HurlCollection collection)
        {
            foreach (string line in lines)
            {
                IHurlSetting? hurlSetting = _settingParser.Parse(line);
                if (hurlSetting != null)
                {
                    collection.CollectionSettings.Add(hurlSetting);
                }
            }
        }


        /// <summary>
        /// Deserializes the locations part of the collection
        /// </summary>
        /// <param name="lines">Plain text lines of a CollectionSectionType.AdditionalLocations section container</param>
        /// <param name="collection">Target collection</param>
        private void DeserializeAdditionalLocationsSection(List<string> lines, HurlCollection collection)
        {
            collection.AdditionalLocations = lines.Where(x => !string.IsNullOrEmpty(x)).Select(x => new AdditionalLocation(x.ConvertDirectorySeparator(), collection)).ToObservableCollection();
        }

        /// <summary>
        /// Deserializes the general part of the collection
        /// </summary>
        /// <param name="lines">Plain text lines of a CollectionSectionType.General section container</param>
        /// <param name="collection">Target collection</param>
        private void DeserializeGeneralSection(List<string> lines, HurlCollection collection)
        {
            foreach (string line in lines)
            {
                string nameKey = $"{SECTION_GENERAL_NAME_KEY}=";
                string excludeRootKey = $"{SECTION_GENERAL_EXCLUDE_ROOT_KEY}=";
                if (line.StartsWith(nameKey))
                {
                    collection.Name = line.Split("=").Get(1)?.DecodeUrl() ?? string.Empty;
                }
                else if (line.StartsWith(excludeRootKey))
                {
                    collection.ExcludeRootDirectory = Convert.ToBoolean(line.Split("=").Get(1));
                }
            }
        }

        /// <summary>
        /// Splits the file content into sections
        /// </summary>
        /// <param name="lines">full collection file content</param>
        /// <returns>A list of containers with corresponding lines</returns>
        private List<HurlCollectionSectionContainer> SplitIntoSections(string[] lines)
        {
            List<HurlCollectionSectionContainer> sections = new List<HurlCollectionSectionContainer>();
            HurlCollectionSectionContainer currentSection = new HurlCollectionSectionContainer();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Create a new section on every known header, then fill in the lines
                switch (line.Trim())
                {
                    case SECTION_FILE_SETTINGS_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlCollectionSectionContainer(CollectionSectionType.FileSettings);
                        break;
                    case SECTION_ADDITIONALLOCATIONS_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlCollectionSectionContainer(CollectionSectionType.AdditionalLocations);
                        break;
                    case SECTION_GENERAL_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlCollectionSectionContainer(CollectionSectionType.General);
                        break;
                    case SECTION_COLLECTION_SETTINGS_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlCollectionSectionContainer(CollectionSectionType.CollectionSettings);
                        break;
                    case SECTION_FOLDER_SETTINGS_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlCollectionSectionContainer(CollectionSectionType.FolderSettings);
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

            return sections.Where(x => x.Type != CollectionSectionType.None).ToList();
        }

        /// <summary>
        /// Deserializes a .hurlc file
        /// </summary>
        /// <param name="filePath">Path to the .hurlc file</param>
        /// <returns>The deserialized colleciton</returns>
        public async Task<HurlCollection> DeserializeFileAsync(string filePath, Encoding encoding)
        {
            string fileContent = await File.ReadAllTextAsync(filePath, encoding);
            return this.Deserialize(fileContent, filePath);
        }

        /// <summary>
        /// Serializes a collection as a .hurlc formatted string
        /// </summary>
        /// <param name="collection">The collection object</param>
        /// <returns>The deserialized collection</returns>
        public string Serialize(HurlCollection collection)
        {
            StringBuilder builder = new StringBuilder();

            // General section
            builder.AppendLine(SECTION_GENERAL_HEADER);
            builder.AppendLine($"{SECTION_GENERAL_NAME_KEY}={collection.Name.EncodeUrl()}");
            builder.AppendLine($"{SECTION_GENERAL_EXCLUDE_ROOT_KEY}={collection.ExcludeRootDirectory}");
            builder.AppendLine();

            // AdditionalLocations section
            builder.AppendLine(SECTION_ADDITIONALLOCATIONS_HEADER);
            foreach (AdditionalLocation location in collection.AdditionalLocations)
            {
                builder.AppendLine($"{location.Path}");
            }
            builder.AppendLine();

            // Collection settings section
            builder.AppendLine(SECTION_COLLECTION_SETTINGS_HEADER);
            foreach (IHurlSetting collectionSetting in collection.CollectionSettings)
            {
                builder.AppendLine(collectionSetting.GetConfigurationString());
            }
            builder.AppendLine();

            // Folder settings section
            foreach (HurlFolder hurlFolder in collection.FolderSettings)
            {
                builder.AppendLine(SECTION_FOLDER_SETTINGS_HEADER);
                builder.AppendLine($"{SECTION_FOLDER_SETTINGS_LOCATION_KEY}={hurlFolder.FolderLocation}");
                foreach (IHurlSetting folderSetting in hurlFolder.FolderSettings)
                {
                    builder.AppendLine(folderSetting.GetConfigurationString());
                }
                builder.AppendLine();
            }

            // File settings section
            foreach (HurlFile hurlFile in collection.FileSettings)
            {
                builder.AppendLine(SECTION_FILE_SETTINGS_HEADER);
                builder.AppendLine($"{SECTION_FILE_SETTINGS_LOCATION_KEY}={hurlFile.FileLocation}");
                foreach (IHurlSetting fileSetting in hurlFile.FileSettings)
                {
                    builder.AppendLine(fileSetting.GetConfigurationString());
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Serializes a collection as a .hurlc file
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task SerializeFileAsync(HurlCollection collection, string filePath, Encoding encoding)
        {
            string serializedContent = await Task.Run(() => this.Serialize(collection));
            await File.WriteAllTextAsync(filePath, serializedContent, encoding);
        }
    }
}
