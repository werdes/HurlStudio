using HurlUI.Collections.Model.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Collections.Utility
{
    public class IniEnvironmentSerializer : IEnvironmentSerializer
    {
        public HurlEnvironment Deserialize(string environmentContent)
        {
            throw new NotImplementedException();
        }

        public Task<HurlEnvironment> DeserializeFileAsync(string filePath, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public string Serialize(HurlEnvironment environment)
        {
            throw new NotImplementedException();
        }

        public Task SerializeFileAsync(HurlEnvironment environment, string filePath, Encoding encoding)
        {
            throw new NotImplementedException();
        }
    }
}
