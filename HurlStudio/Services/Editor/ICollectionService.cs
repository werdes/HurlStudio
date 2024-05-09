using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using HurlStudio.Model.CollectionContainer;
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
        Task<CollectionContainer> GetCollectionContainerAsync(HurlCollection collection);
        Task<ObservableCollection<CollectionContainer>> GetCollectionContainersAsync();
        Task StoreCollectionAsync(HurlCollection collection, string collectionLocation);

    }
}
