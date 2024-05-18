using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class RetryArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--retry";
        private readonly int _maxRetries;

        public RetryArgument(int maxRetries) => _maxRetries = maxRetries;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _maxRetries.ToString()
            };
        }
    }
}
