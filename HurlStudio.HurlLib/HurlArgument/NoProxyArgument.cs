namespace HurlStudio.HurlLib.HurlArgument
{
    public class NoProxyArgument : IHurlArgument, IMergeableArgument<string>
    {
        private const string NAME_ARGUMENT = "--noproxy";
        private const string OPTION_SEPARATOR = ",";
        private readonly List<string> _hosts;

        public NoProxyArgument(List<string> hosts) => _hosts = hosts;

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                string.Join(OPTION_SEPARATOR, _hosts)
            };
        }
        
        /// <summary>
        /// Returns the mergeable attributes, e.g. the proxy hosts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetMergeableValues()
        {
            return _hosts;
        }
    }
}
