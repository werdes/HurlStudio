using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Extensions
{
    public static class PathExtensions
    {
        private static readonly string[] INVALID_DIRECTORY_NAMES =
        [
            "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "COM0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "LPT0"
        ];

        public static bool IsChildOfDirectory(string directory, string parentDirectory)
        {
            bool isChild = false;

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            DirectoryInfo parentDirectoryInfo = new DirectoryInfo(parentDirectory);

            while (directoryInfo != null && directoryInfo.Parent != null)
            {
                if (directoryInfo.Parent.FullName == parentDirectoryInfo.FullName)
                {
                    isChild = true;
                    break;
                }
                else
                {
                    directoryInfo = directoryInfo.Parent;
                }
            }

            return isChild;
        }

        /// <summary>
        /// Returns a valid filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetValidFileName(this string fileName)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char invalidFileNameChar in invalidFileNameChars)
            {
                fileName = fileName.Replace(invalidFileNameChar, '_');
            }

            fileName = fileName.Trim('_').Squash('_');

            return fileName;
        }

        /// <summary>
        /// Returns a valid directory name
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        public static string GetValidDirectoryName(this string directoryName)
        {
            while (INVALID_DIRECTORY_NAMES.Contains(directoryName.ToUpper()))
            {
                directoryName += '_';
            }

            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char invalidFileNameChar in invalidFileNameChars)
            {
                directoryName = directoryName.Replace(invalidFileNameChar, '_');
            }
            directoryName = directoryName.Squash('_');

            return directoryName;
        }
    }
}