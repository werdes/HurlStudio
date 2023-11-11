using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class ResolveArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--resolve";
        private string _resolveString;

        public ResolveArgument(string resolveString) => this._resolveString = resolveString;
        public ResolveArgument(string host, ushort port, string address) => 
            this._resolveString = $"{host}:{port}:{address}";

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._resolveString
            };
        }
    }
}
