using HurlStudio.Collections.Model;
using HurlStudio.Model.HurlContainers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HurlStudio.Model.HurlFileTemplates;

namespace HurlStudio.Services.Editor
{
    public interface ICollectionService
    {
        Task<HurlCollection> GetCollectionAsync(string collectionLocation);
        Task<IEnumerable<HurlCollection>> GetCollectionsAsync();
        Task<HurlCollectionContainer> GetCollectionContainerAsync(HurlCollection collection);
        Task<HurlCollectionContainer> SetCollectionContainerAsync(HurlCollectionContainer container, HurlCollection collection);
        Task<ObservableCollection<HurlCollectionContainer>> GetCollectionContainersAsync();
        Task StoreCollectionAsync(HurlCollection collection, string collectionLocation);
        Task<bool> CreateCollection(HurlCollection collection);
        
        /// <summary>
        /// Creates a file from a template inside the given folder
        /// </summary>
        /// <param name="parentFolderContainer">The folder the file will be created in</param>
        /// <param name="fileTemplate">The template that contains the settings selected by the user</param>
        /// <param name="fileName">The filename selected by the user</param>
        /// <returns>Tuple:
        ///     Value 1: true, of the file was created successfully
        ///     Value 2: the absolute file path of the file, if it was created
        /// </returns>
        Task<(bool, string?)> CreateFile(HurlFolderContainer parentFolderContainer, HurlFileTemplateContainer fileTemplate, string fileName);
        
        /// <summary>
        /// Creates a file from a template inside the root folder of the given collection
        /// </summary>
        /// <param name="collectionContainer">The collection the file will be created in</param>
        /// <param name="fileTemplate">The template that contains the settings selected by the user</param>
        /// <param name="fileName">The filename selected by the user</param>
        /// <returns>Tuple:
        ///     Value 1: true, of the file was created successfully
        ///     Value 2: the absolute file path of the file, if it was created
        /// </returns>
        Task<(bool, string?)> CreateFile(HurlCollectionContainer collectionContainer, HurlFileTemplateContainer fileTemplate, string fileName);
        
        /// <summary>
        /// Creates a folder at the given location
        /// ( As the empty folder doesn't have any initial settings,
        ///   no changes to the collection have to be made, so this
        ///   is basically a wrapper for Directory.CreateDirectory().
        ///   Nevertheless, for future additions, this is part of the
        ///   service. )
        /// </summary>
        /// <param name="collectionContainer">The collection the folder will be placed under (unused atm)</param>
        /// <param name="absolutePath">The absolute path of the folder to be created</param>
        /// <returns>true, if the folder was created</returns>
        Task<bool> CreateFolder(HurlCollectionContainer collectionContainer, string absolutePath);
    }
}
