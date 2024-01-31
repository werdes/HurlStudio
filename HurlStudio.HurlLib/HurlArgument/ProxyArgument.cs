using HurlStudio.Common.Enums;
using HurlStudio.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class ProxyArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--proxy";
        private string _proxyString;

        public ProxyArgument(string proxyString) => _proxyString = proxyString;
        public ProxyArgument(ProxyProtocol? protocol, string? host, ushort? port) =>
            _proxyString = $"{protocol?.ToString().ToLower()}://{host}:{port}";
        public ProxyArgument(ProxyProtocol? protocol, string? host, ushort? port, string? user, string? password) =>
                    _proxyString = $"{protocol?.ToString().ToLower()}://{user?.UrlEncode()}:{password?.UrlEncode()}@{host}:{port}";


        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _proxyString
            };
        }
    }
}
