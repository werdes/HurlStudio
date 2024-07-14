using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class CustomStringArgument : IHurlArgument
    {
        private readonly string[] _values;

        public CustomStringArgument(string[] values)
        {
            _values = values;
        }

        public string[] GetCommandLineArguments()
        {
            return _values;
        }
    }
}
