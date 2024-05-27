using HurlStudio.Collections.Model;
using System.Text;

namespace HurlStudio.Collections.Utility
{
    public interface ICollectionSerializer
    {
        HurlCollection Deserialize(string collectionContent, string filePath);
        Task<HurlCollection> DeserializeFileAsync(string filePath, Encoding encoding);
        string Serialize(HurlCollection collection);
        Task SerializeFileAsync(HurlCollection collection, string filePath, Encoding encoding);
    }
}