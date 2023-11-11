using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class ErrorFormatArgument : IHurlArgument
    {
        public enum Format
        {
            Short,
            Long
        }

        private const string NAME_ARGUMENT = "--error-format";
        private Format _format;

        public ErrorFormatArgument(Format format) => this._format = format;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._format.ToString()
            };
        }
    }
}
