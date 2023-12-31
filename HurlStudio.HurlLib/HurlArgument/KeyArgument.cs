﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class KeyArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--key";
        private string _keyFileName;

        public KeyArgument(string keyFileName) => _keyFileName = keyFileName;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                _keyFileName
            };
        }
    }
}
