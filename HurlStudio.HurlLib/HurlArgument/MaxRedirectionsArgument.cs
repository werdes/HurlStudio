using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class MaxRedirectionsArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--max-redirs";
        private int _maxRedirections;

        public MaxRedirectionsArgument(int maxRedirections) => _maxRedirections = maxRedirections;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _maxRedirections.ToString()
            };
        }
    }
}
