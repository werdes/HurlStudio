using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.HurlLib.HurlArgument
{
    public class NoProxyArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--noproxy";
        private string _hosts;

        public NoProxyArgument(string file) => this._hosts = file;
        public NoProxyArgument(IEnumerable<string> hosts) => this._hosts = string.Join(",", hosts);

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._hosts
            };
        }
    }
}
