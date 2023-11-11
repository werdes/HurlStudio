using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class ToEntryArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--to-entry";
        private int _entryNumber;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="entryNumber">execute file up to this request, ignore the remaining requests</param>
        public ToEntryArgument(int entryNumber) => this._entryNumber = entryNumber;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._entryNumber.ToString()
            };
        }
    }
}
