using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common
{
    public static class GlobalConstants
    {
        public const string APPLICATION_DIRECTORY_NAME = "HurlStudio";
        public const string ENVIRONMENTS_DIRECTORY_NAME = "environments";
        public const string LOG_DIRECTORY_NAME = "log";
        public const string USERSETTINGS_JSON_FILE_NAME = "settings.json";
        public const string UISTATE_JSON_FILE_NAME = "uistate.json";

        public const string HURL_FILE_EXTENSION = ".hurl";
        public const string COLLECTION_FILE_EXTENSION = ".hurlc";
        public const string ENVIRONMENT_FILE_EXTENSION = ".hurle";

        public const string GRAMMAR_HURL_NAME = "hurl";

        public const int DEFAULT_FILE_HISTORY_LENGTH = 5;
        public const int ENVIRONMENT_EXITCODE_ERROR = 1;
    }
}
