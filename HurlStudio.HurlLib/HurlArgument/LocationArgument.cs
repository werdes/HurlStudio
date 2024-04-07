namespace HurlStudio.HurlLib.HurlArgument
{
    public class LocationArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--location";

        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT
            };
        }
    }
}
