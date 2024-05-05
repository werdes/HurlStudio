using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class MaxFilesizeArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--max-filesize";
        private uint _maxFilesize;

        public MaxFilesizeArgument(uint maxFilesize) => _maxFilesize = maxFilesize;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _maxFilesize.ToString()
            };
        }
    }
}
