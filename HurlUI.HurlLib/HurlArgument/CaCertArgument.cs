using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.HurlLib.HurlArgument
{
    public class CaCertArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--cacert";
        private string _file;

        public CaCertArgument(string file) => this._file = file;

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
