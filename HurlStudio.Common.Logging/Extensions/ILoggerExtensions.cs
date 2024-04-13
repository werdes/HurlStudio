using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HurlStudio.Common.Logging.Extensions
{
    public static class ILoggerExtensions
    {
        private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Converters = { new JsonStringEnumConverter() }
        };

        public static void LogObject(this ILogger logger, object obj, LogLevel logLevel = LogLevel.Trace)
        {
            string json = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
            string[] lines = json.Split('\n');
            foreach(string line in lines)
            {
                logger.Log(logLevel, line);
            }
        }
    }
}
