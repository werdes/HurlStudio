using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class RetryIntervalArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--retry-interval";
        private uint _retryInterval;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="retryInterval">Duration between each retry in ms</param>
        public RetryIntervalArgument(uint retryInterval) => _retryInterval = retryInterval;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="retryInterval">Duration between each retry</param>
        public RetryIntervalArgument(TimeSpan retryInterval) => _retryInterval = (uint)retryInterval.TotalMilliseconds;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _retryInterval.ToString()
            };
        }
    }
}
