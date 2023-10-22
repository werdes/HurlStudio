using HurlUI.Collections.Model.Collection;
using HurlUI.Collections.Model.Serializer;
using HurlUI.Collections.Settings;
using HurlUI.Common.Enums;
using HurlUI.Common.Extensions;
using System.Text;

namespace HurlUI.Collections.Utility
{
    public class IniCollectionSerializer : ICollectionSerializer
    {
        private ISettingParser _settingParser;

        public IniCollectionSerializer(ISettingParser settingParser)
        {
            this._settingParser = settingParser;
        }


        public const string FILE_EXTENSION = ".hurlcol";

        private const string SECTION_GENERAL_HEADER = "[Collection]";
        private const string SECTION_GENERAL_NAME_KEY = "name";
        private const string SECTION_LOCATIONS_HEADER = "[Locations]";
        private const string SECTION_COLLECTION_SETTINGS_HEADER = "[CollectionSettings]";
        private const string SECTION_FILE_SETTINGS_HEADER = "[FileSettings]";
        private const string SECTION_FILE_SETTINGS_LOCATION_KEY = "location";


        /// <summary>
        /// Deserializes a .hurlcol formatted string
        /// </summary>
        /// <param name="collectionContent">.hurlcol file content</param>
        /// <returns></returns>
        public HurlCollection Deserialize(string collectionContent)
        {
            string[] lines = collectionContent.Split(Environment.NewLine);
            List<HurlCollectionSectionContainer> collectionSections = SplitIntoSections(lines);
            HurlCollection collection = new HurlCollection();

            foreach (HurlCollectionSectionContainer sectionContainer in collectionSections)
            {
                switch (sectionContainer.Type)
                {
                    case CollectionSectionType.General:
                        DeserializeGeneralSection(sectionContainer.Lines, ref collection);
                        break;
                    case CollectionSectionType.Locations:
                        DeserializeLocationsSection(sectionContainer.Lines, ref collection);
                        break;
                    case CollectionSectionType.CollectionSettings:
                        DeserializeCollectionSettingsSection(sectionContainer.Lines, ref collection);
                        break;
                    case CollectionSectionType.FileSettings:
                        DeserializeFileSettingsSection(sectionContainer.Lines, ref collection);
                        break;
                }
            }

            return collection;
        }

        /// <summary>
        /// Deserializes the collection settings part of the collection
        /// </summary>
        /// <param name="lines">plain text lines of a CollectionSectionType.FileSettings section container</param>
        /// <param name="collection">Target collection</param>
        private void DeserializeFileSettingsSection(List<string> lines, ref HurlCollection collection)
        {
            HurlFile hurlFile = new HurlFile();
            foreach (string line in lines)
            {
                string locationKey = SECTION_FILE_SETTINGS_LOCATION_KEY + "=";
                if (line.StartsWith(locationKey))
                {
                    // .hurl file location
                    hurlFile.FileLocation = line.Split('=').Get(1) ?? string.Empty;
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

            collection.FileSettings.Add(hurlFile);
        }

        /// <summary>
        /// Deserializes the collection settings part of the collection
        /// </summary>
        /// <param name="lines">plain text lines of a CollectionSectionType.CollectionSettings section container</param>
        /// <param name="collection">Target collection</param>
        private void DeserializeCollectionSettingsSection(List<string> lines, ref HurlCollection collection)
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
        /// <param name="lines">Plain text lines of a CollectionSectionType.Locations section container</param>
        /// <param name="collection">Target collection</param>
        private void DeserializeLocationsSection(List<string> lines, ref HurlCollection collection)
        {
            collection.Locations = lines.Where(x => !string.IsNullOrEmpty(x)).ToList();
        }

        /// <summary>
        /// Deserializes the general part of the collection
        /// </summary>
        /// <param name="lines">Plain text lines of a CollectionSectionType.General section container</param>
        /// <param name="collection">Target collection</param>
        private void DeserializeGeneralSection(List<string> lines, ref HurlCollection collection)
        {
            foreach (string line in lines)
            {
                string nameKey = $"{SECTION_GENERAL_NAME_KEY}=";
                if (line.StartsWith(nameKey))
                {
                    collection.Name = line.Split("=").Get(1) ?? string.Empty;
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
                    case SECTION_LOCATIONS_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlCollectionSectionContainer(CollectionSectionType.Locations);
                        break;
                    case SECTION_GENERAL_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlCollectionSectionContainer(CollectionSectionType.General);
                        break;
                    case SECTION_COLLECTION_SETTINGS_HEADER:
                        sections.Add(currentSection);
                        currentSection = new HurlCollectionSectionContainer(CollectionSectionType.CollectionSettings);
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
        /// Deserializes a .hurlcol file
        /// </summary>
        /// <param name="filePath">Path to the .hurlcol file</param>
        /// <returns>The deserialized colleciton</returns>
        public async Task<HurlCollection> DeserializeFileAsync(string filePath, Encoding encoding)
        {
            string fileContent = await File.ReadAllTextAsync(filePath, encoding);
            return Deserialize(fileContent);
        }

        /// <summary>
        /// Serializes a collection as a .hurlcol formatted string
        /// </summary>
        /// <param name="collection">The collection object</param>
        /// <returns>The deserialized collection</returns>
        public string Serialize(HurlCollection collection)
        {
            StringBuilder builder = new StringBuilder();

            // General section
            builder.AppendLine(SECTION_GENERAL_HEADER);
            builder.AppendLine($"{SECTION_GENERAL_NAME_KEY}={collection.Name.UrlEncode()}");
            builder.AppendLine();

            // Locations section
            builder.AppendLine(SECTION_LOCATIONS_HEADER);
            foreach (string location in collection.Locations)
            {
                builder.AppendLine($"{location}");
            }
            builder.AppendLine();

            // Collection settings section
            builder.AppendLine(SECTION_COLLECTION_SETTINGS_HEADER);
            foreach (IHurlSetting collectionSetting in collection.CollectionSettings)
            {
                builder.AppendLine(collectionSetting.GetConfigurationString());
            }
            builder.AppendLine();

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
        /// Serializes a collection as a .hurlcol file
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task SerializeFileAsync(HurlCollection collection, string filePath, Encoding encoding)
        {
            string serializedContent = Serialize(collection);
            await File.WriteAllTextAsync(filePath, serializedContent, encoding);
        }
    }
}
