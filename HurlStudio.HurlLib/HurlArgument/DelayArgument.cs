using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class DelayArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--delay";
        private uint _delay;

        public DelayArgument(uint delay) => _delay = delay;
        public DelayArgument(TimeSpan delay) => _delay = (uint)delay.TotalMilliseconds;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _delay.ToString()
            };
        }
    }
}
