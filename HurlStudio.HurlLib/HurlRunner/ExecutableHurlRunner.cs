using HurlStudio.HurlLib.HurlArgument;
using HurlStudio.HurlLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlRunner
{
    public class ExecutableHurlRunner : BaseHurlRunner, IHurlRunner
    {
        private string _executablePath;

        /// <summary>
        /// Constructor with path for executable
        /// </summary>
        /// <param name="executablePath">path to hurl executable</param>
        public ExecutableHurlRunner(string executablePath) => this._executablePath = executablePath;

        /// <summary>
        /// Runs hurl via executable
        /// </summary>
        /// <param name="file">hurl file path</param>
        /// <param name="arguments">arguments for process call</param>
        /// <returns>A HurlRunnerResult object, containing ExitCode and StdOut</returns>
        public override async Task<HurlRunnerResult> RunHurlAsync(string file, IEnumerable<IHurlArgument> arguments) =>
            await base.Run(file, false, arguments);
    }
}
