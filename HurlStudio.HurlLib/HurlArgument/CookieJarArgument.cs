using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class CookieJarArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--cookie-jar";
        private string _file;

        public CookieJarArgument(string file) => _file = file;

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
