using HurlStudio.HurlLib.HurlArgument;
using HurlStudio.HurlLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlRunner
{
    public interface IHurlRunner
    {
        Task<HurlRunnerResult> RunHurlAsync(string file, IEnumerable<IHurlArgument> arguments);
        Task<HurlRunnerResult> RunHurlAsync(string file, params IHurlArgument[] arguments);
    }
}
