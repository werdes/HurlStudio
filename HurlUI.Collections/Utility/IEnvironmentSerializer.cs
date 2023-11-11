using HurlUI.Collections.Model.Collection;
using HurlUI.Collections.Model.Environment;
using System.Text;

namespace HurlUI.Collections.Utility
{
    public interface IEnvironmentSerializer
    {
        HurlEnvironment Deserialize(string environmentContent);
        Task<HurlEnvironment> DeserializeFileAsync(string filePath, Encoding encoding);
        string Serialize(HurlEnvironment environment);
        Task SerializeFileAsync(HurlEnvironment environment, string filePath, Encoding encoding);
    }
}