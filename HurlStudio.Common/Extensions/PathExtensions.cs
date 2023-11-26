using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Extensions
{
    public static class PathExtensions
    {
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
    }
}
