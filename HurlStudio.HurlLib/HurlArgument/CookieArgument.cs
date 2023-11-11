using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class CookieArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--cookie";
        private string _file;

        public CookieArgument(string file) => this._file = file;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._file
            };
        }
    }
}
