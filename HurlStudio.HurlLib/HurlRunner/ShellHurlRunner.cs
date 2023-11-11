using HurlStudio.HurlLib.HurlArgument;
using HurlStudio.HurlLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlRunner
{
    public class ShellHurlRunner : BaseHurlRunner, IHurlRunner
    {
        private const string DEFAULT_HURL_COMMAND = "hurl";
        private string _command;

        public ShellHurlRunner() => _command = DEFAULT_HURL_COMMAND;
        public ShellHurlRunner(string command) => _command = command;

        /// <summary>
        /// Runs hurl via shell command
        /// </summary>
        /// <param name="file">hurl file path</param>
        /// <param name="arguments">arguments for process call</param>
        /// <returns>A HurlRunnerResult object, containing ExitCode and StdOut</returns>
        public override async Task<HurlRunnerResult> RunHurlAsync(string file, IEnumerable<IHurlArgument> arguments) =>
            await base.Run(_command, true, arguments);
    }
}
