using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.HurlLib.HurlArgument
{
    public class ConnectTimeoutArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--connect-timeout";
        private int _seconds;

        public ConnectTimeoutArgument(int seconds) => this._seconds = seconds;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._seconds.ToString()
            };
        }
    }
}
