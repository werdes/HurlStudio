using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.HurlLib.HurlArgument
{
    public class ClientCertificateArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--cert";
        private string _certificate;

        public ClientCertificateArgument(string certificate) => this._certificate = certificate;
        public ClientCertificateArgument(string certificate, string password) => this._certificate = $"{certificate}:{password}";

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._certificate
            };
        }
    }
}
