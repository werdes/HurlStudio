using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class ReportTapArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--report-tap";
        private string _file;

        public ReportTapArgument(string file) => _file = file;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _file
            };
        }
    }
}
