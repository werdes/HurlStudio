using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class AwsSigV4Argument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--aws-sigv4";
        private const string VALUE_SEPARATOR = ":";

        private string _provider1;
        private string _provider2;
        private string _region;
        private string _service;

        public AwsSigV4Argument(string provider1, string provider2, string region, string service)
        {
            _provider1 = provider1;
            _provider2 = provider2;
            _region = region;
            _service = service;
        }

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            List<string> values = new List<string>();
            values.AddIfNotNull(_provider1);
            values.AddIfNotNull(_provider2);
            values.AddIfNotNull(_region);
            values.AddIfNotNull(_service);

            return new string[]
            {
                NAME_ARGUMENT,
                string.Join(VALUE_SEPARATOR, values)
            };
        }
    }
}
