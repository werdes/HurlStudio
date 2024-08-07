﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Web;

namespace HurlStudio.Common.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex NORMALIZE_REGEX = new Regex("[^A-Za-z0-9 ]", RegexOptions.Compiled);

        /// <summary>
        /// Encodes a string as URL parameter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeUrl(this string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        /// <summary>
        /// Decodes an URL parameter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecodeUrl(this string value)
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

            if (replacableChar.HasValue)
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

        /// <summary>
        /// Converts a string into a representing sha256 hash
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSha256Hash(this string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] buffer = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(buffer, 0, buffer.Length).Replace("-", "");
            }
        }

        /// <summary>
        /// Encodes a string as Base64
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncodeBase64(this string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decodes a string from Base64
        /// </summary>
        /// <param name="base64Text"></param>
        /// <returns></returns>
        public static string DecodeBase64(this string base64Text)
        {
            byte[] bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Normalizes a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToNormalized(this string text)
        {
            return NORMALIZE_REGEX.Replace(text, string.Empty).ToLower();
        }

        /// <summary>
        /// Returns true, if a given string is included in the list of params
        /// </summary>
        /// <param name="query"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static bool IsContainedInAnyNormalized(this string query, params string?[] includes)
        {
            string queryNormalized = query.ToNormalized();
            List<string> normalizedParams = includes.Where(x => !string.IsNullOrWhiteSpace(x))
                                                    .Select(x => x.ToNormalized()).ToList();
            bool isContained = normalizedParams.Any(x => x.Contains(queryNormalized));
            return isContained;  
        }

        /// <summary>
        /// Squashes sequences of the given char <paramref name="squashChar"/> down to a single one
        /// </summary>
        /// <param name="text">Text the given char has to be squashed from</param>
        /// <param name="squashChar">Char to be squashed</param>
        /// <returns>squashed text</returns>
        public static string Squash(this string text, char squashChar)
        {
            Regex regex = new Regex("[" + squashChar + "]{2,}");
            return regex.Replace(text, squashChar.ToString());
        }
    }
}
