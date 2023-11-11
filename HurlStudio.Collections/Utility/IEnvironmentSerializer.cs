using HurlStudio.Collections.Model.Collection;
using HurlStudio.Collections.Model.Environment;
using System.Text;

namespace HurlStudio.Collections.Utility
{
    public interface IEnvironmentSerializer
    {
        HurlEnvironment Deserialize(string environmentContent);
        Task<HurlEnvironment> DeserializeFileAsync(string filePath, Encoding encoding);
        string Serialize(HurlEnvironment environment);
        Task SerializeFileAsync(HurlEnvironment environment, string filePath, Encoding encoding);
    }
}