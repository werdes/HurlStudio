using HurlUI.Collections.Model.Collection;
using System.Text;

namespace HurlUI.Collections.Utility
{
    public interface ICollectionSerializer
    {
        HurlCollection Deserialize(string collectionContent);
        Task<HurlCollection> DeserializeFileAsync(string filePath, Encoding encoding);
        string Serialize(HurlCollection collection);
        Task SerializeFileAsync(HurlCollection collection, string filePath, Encoding encoding);
    }
}