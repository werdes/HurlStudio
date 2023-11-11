using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class MaxTimeArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--max-time";
        private int _maxSeconds;

        public MaxTimeArgument(int maxSeconds) => this._maxSeconds = maxSeconds;
        public MaxTimeArgument(TimeSpan maxTime) => this._maxSeconds = (int)maxTime.TotalSeconds;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._maxSeconds.ToString()
            };
        }
    }
}
