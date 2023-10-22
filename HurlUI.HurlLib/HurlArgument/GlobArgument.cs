﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.HurlLib.HurlArgument
{
    public class GlobArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--glob";
        private string _glob;

        public GlobArgument(string glob) => this._glob = glob;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._glob
            };
        }
    }
}
