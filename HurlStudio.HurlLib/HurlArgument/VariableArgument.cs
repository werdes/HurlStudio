using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class VariableArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--variable";
        private string _variableString;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable value</param>
        public VariableArgument(string name, string value) => this._variableString = $"{name}={value}";

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._variableString
            };
        }
    }
}
