using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Common.Utility
{
    public static class OSUtility
    {
        /// <summary>
        /// Opens the folder containing a given file or folder in the systems' file explorer
        /// see https://stackoverflow.com/questions/73409227/from-dotnet-how-to-open-file-in-containing-folder-in-the-linux-file-manager
        /// </summary>
        /// <param name="path"></param>
        public static void RevealFileInExplorer(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using Process fileOpener = new Process();
                    fileOpener.StartInfo.FileName = "explorer";
                    fileOpener.StartInfo.Arguments = "/select," + path + "\"";
                    fileOpener.Start();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    using Process fileOpener = new Process();
                    fileOpener.StartInfo.FileName = "explorer";
                    fileOpener.StartInfo.Arguments = "-R " + path;
                    fileOpener.Start();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    using Process dbusShowItemsProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "dbus-send",
                            Arguments = "--print-reply --dest=org.freedesktop.FileManager1 /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems array:string:\"file://" + path + "\" string:\"\"",
                            UseShellExecute = true
                        }
                    };
                    dbusShowItemsProcess.Start();
                }
            }
        }
    }
}
