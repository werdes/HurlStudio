using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.Model
{
    /// <summary>
    /// see https://hurl.dev/docs/manual.html#exit-codes
    /// </summary>
    public enum HurlExitCode
    {
        Success = 0,
        FailedToParseCommandLineOptions = 1,
        InputFileParsingError = 2,
        RuntimeError = 3,
        AssertError = 4
    }
}
