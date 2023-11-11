using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class UserAgentArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--user-agent";
        private string _userAgent;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="_userAgent">UserAgent string</param>
        public UserAgentArgument(string _userAgent) => this._userAgent = _userAgent;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._userAgent
            };
        }
    }
}
