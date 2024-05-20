using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HurlStudio.Common.Extensions
{
    public static class ILoggerExtensions
    {
        private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// Logger extension for logging any object as json
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="obj"></param>
        /// <param name="logLevel"></param>
        public static void LogObject(this ILogger logger, object obj, LogLevel logLevel = LogLevel.Trace)
        {
            string json = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
            string[] lines = json.Split('\n');
            foreach(string line in lines)
            {
                logger.Log(logLevel, line);
            }
        }

        /// <summary>
        /// Logger extension for exceptions with caller name
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="caller"></param>
        public static void LogException(this ILogger logger, Exception exception, [CallerMemberName] string caller = "")
        {
            logger.LogCritical(exception, caller);
        }
    }
}
