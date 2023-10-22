using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.HurlLib.HurlArgument
{
    public class AwsSigV4Argument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--aws-sigv4";
        private string _service;

        public AwsSigV4Argument(string service) => this._service = service;
        
        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._service
            };
        }
    }
}
