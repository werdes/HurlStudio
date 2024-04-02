using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class NoProxyArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--noproxy";
        private const string OPTION_SEPARATOR = ",";
        private string _hosts;

        public NoProxyArgument(string file) => _hosts = file;
        public NoProxyArgument(IEnumerable<string> hosts) => _hosts = string.Join(OPTION_SEPARATOR, hosts);

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _hosts
            };
        }
    }
}
