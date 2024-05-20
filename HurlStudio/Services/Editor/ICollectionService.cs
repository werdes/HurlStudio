using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Model.HurlContainers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public interface ICollectionService
    {
        Task<HurlCollection> GetCollectionAsync(string collectionLocation);
        Task<IEnumerable<HurlCollection>> GetCollectionsAsync();
        Task<HurlCollectionContainer> GetCollectionContainerAsync(HurlCollection collection);
        Task<ObservableCollection<HurlCollectionContainer>> GetCollectionContainersAsync();
        Task StoreCollectionAsync(HurlCollection collection, string collectionLocation);

    }
}
