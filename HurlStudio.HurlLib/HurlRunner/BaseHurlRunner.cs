using HurlStudio.HurlLib.HurlArgument;
using HurlStudio.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HurlStudio.HurlLib.Model;

namespace HurlStudio.HurlLib.HurlRunner
{
    public abstract class BaseHurlRunner : IHurlRunner
    {
        /// <summary>
        /// Runner command, agnostic of which version of hurl is run (shell, executable)
        ///  has to be specified from a subclass
        /// </summary>
        /// <param name="file"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public abstract Task<HurlRunnerResult> RunHurlAsync(string file, IEnumerable<IHurlArgument> arguments);

        /// <summary>
        /// Params version of RunHurlAsync
        /// </summary>
        /// <param name="file"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public async Task<HurlRunnerResult> RunHurlAsync(string file, params IHurlArgument[] arguments) =>
           await this.RunHurlAsync(file, arguments);

        /// <summary>
        /// Starts the hurl process, waits for its completion, and returns its output
        /// </summary>
        /// <param name="file">Either hurl executable location or hurl command (see <paramref name="useShellExecute"/>)</param>
        /// <param name="useShellExecute">Process is run as a shell command if true (see <paramref name="file"/>)</param>
        /// <param name="arguments">list of arguments</param>
        /// <returns>A HurlRunnerResult object, containing ExitCode and StdOut</returns>
        protected async Task<HurlRunnerResult> Run(string file, bool useShellExecute, IEnumerable<IHurlArgument> arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = file;
            process.StartInfo.UseShellExecute = useShellExecute;

            // pass .hurl file as first parameter
            process.StartInfo.ArgumentList.Add(file);
            arguments.ForEach(x => x.GetCommandLineArguments().ForEach(y => process.StartInfo.ArgumentList.Add(y)));

            process.Start();
            await process.WaitForExitAsync();

            return new HurlRunnerResult()
            {
                StandardOutput = await process.StandardOutput.ReadToEndAsync(),
                ExitCode = (HurlExitCode)process.ExitCode
            };
        }
    }
}
