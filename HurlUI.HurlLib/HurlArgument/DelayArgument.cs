using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.HurlLib.HurlArgument
{
    public class DelayArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--delay";
        private int _delay;

        public DelayArgument(int delay) => this._delay = delay;
        public DelayArgument(TimeSpan delay) => this._delay = (int)delay.TotalMilliseconds;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._delay.ToString()
            };
        }
    }
}
