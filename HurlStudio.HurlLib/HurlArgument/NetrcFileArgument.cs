using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class NetrcFileArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--netrc-file";
        private string _file;

        public NetrcFileArgument(string file) => _file = file;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[] 
            {
                NAME_ARGUMENT,
                _file
            };
        }
    }
}
