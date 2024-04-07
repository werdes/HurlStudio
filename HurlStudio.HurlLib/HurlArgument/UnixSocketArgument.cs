using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class UnixSocketArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--unix-socket";
        private readonly string _path;

        public UnixSocketArgument(string path) => _path = path;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _path
            };
        }
    }
}
