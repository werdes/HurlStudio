﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class ConnectToArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--connect-to";
        private string _connectToString;

        public ConnectToArgument(string connectToString) => _connectToString = connectToString;
        public ConnectToArgument(string host1, ushort port1, string host2, ushort port2) => _connectToString = $"{host1}:{port1}:{host2}:{port2}";

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _connectToString
            };
        }
    }
}
