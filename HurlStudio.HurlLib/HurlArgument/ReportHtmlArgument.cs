using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class ReportHtmlArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--report-html";
        private string _directory;

        public ReportHtmlArgument(string directory) => this._directory = directory;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._directory
            };
        }
    }
}
