using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HurlStudio.Common.Extensions
{
    public static class StringExtensions
    {
        public static string UrlEncode(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        public static string UrlDecode(this string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        /// <summary>
        /// Converts a path to a platform correct path
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string ConvertDirectorySeparator(this string directory)
        {
            char platformSpecificSeparatorChar = Path.DirectorySeparatorChar;
            char? replacableChar = null;
            switch (platformSpecificSeparatorChar)
            {
                case '\\': replacableChar = '/'; break;
                case '/': replacableChar = '\\'; break;
            }

            if(replacableChar.HasValue)
            {
                return directory.Replace(replacableChar.Value, platformSpecificSeparatorChar);
            }
            return directory;
        }

        /// <summary>
        /// Parses a string to a nullable bool
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool? NullableTryParseBool(this string text)
        {
            bool value;
            return bool.TryParse(text, out value) ? (bool?)value : null;
        }

        /// <summary>
        /// Parses a string to a nullable int
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int? NullableTryParseInt(this string text)
        {
            int value;
            return int.TryParse(text, out value) ? (int?)value : null;
        }
    }
}
