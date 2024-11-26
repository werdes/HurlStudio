using HurlStudio.Collections.Model;
using HurlStudio.Model.HurlContainers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
    }
}
