using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.Model
{
    public class HurlRunnerResult
    {
        public HurlExitCode ExitCode { get; set; }
        public string? StandardOutput { get; set; }
    }
}
