using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.HurlLib.HurlArgument
{
    public class UserArgument : IHurlArgument
    {
        private const string NAME_ARGUMENT = "--user";
        private string _userString;

        /// <summary>
        /// User string for Basic Authentication
        /// </summary>
        /// <param name="userString">String containing user and password in the format of {user}:{password}</param>
        public UserArgument(string userString) => this._userString = userString;

        /// <summary>
        /// User and password for Basic Authentication
        /// </summary>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        public UserArgument(string user, string password) => this._userString = $"{user}:{password}";


        /// <summary>
        /// Returns the arguments
        /// </summary>
        /// <returns>CLI arguments</returns>
        public string[] GetCommandLineArguments()
        {
            return new string[]
            {
                NAME_ARGUMENT,
                this._userString
            };
        }
    }
}
