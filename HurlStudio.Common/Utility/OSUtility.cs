using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HurlStudio.Common.Utility
{
    public static class OSUtility
    {
        private const string ENV_XDG_CURRENT_DESKTOP = "XDG_CURRENT_DESKTOP";
        private const string ENV_DESKTOP_SESSION = "DESKTOP_SESSION";
        private const string ENV_KDE_SESSION_VERSION = "KDE_SESSION_VERSION";
        private const string DE_UNITY = "UNITY";
        private const string DE_DEEPIN = "DEEPIN";
        private const string DE_GNOME = "GNOME";
        private const string DE_CINNAMON = "X-Cinnamon";
        private const string DE_KDE = "KDE";
        private const string DE_PANTHEON = "Pantheon";
        private const string DE_XFCE = "XFCE";
        private const string DE_UKUI = "UKUI";
        private const string DE_LXQT = "LXQt";

        private enum DesktopEnvironment
        {
            Undefined,
            Unity,
            Deepin,
            Gnome,
            Cinnamon,
            KDE4,
            KDE5,
            KDE6,
            Pantheon,
            XFCE,
            UKUI,
            LXQt
        }

        private enum OSXPosixType
        {
            File,
            Folder
        }

        /// <summary>
        /// Opens the folder containing a given file or folder in the systems' file explorer
        /// see https://stackoverflow.com/questions/73409227/from-dotnet-how-to-open-file-in-containing-folder-in-the-linux-file-manager
        /// </summary>
        /// <param name="path"></param>
        public static void RevealPathInExplorer(string path)
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

        /// <summary>
        /// Moves a file to trash
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<bool> MoveFileToTrash(string filePath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await MoveFileToTrashWindows(filePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                await MoveToTrashLinux(filePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                await MoveToTrashOSX(filePath, OSXPosixType.File);
            }

            return !File.Exists(filePath);
        }

        /// <summary>
        /// Moves a folder to trash
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static async Task<bool> MoveFolderToTrash(string folderPath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await MoveFolderToTrashWindows(folderPath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                await MoveToTrashLinux(folderPath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                await MoveToTrashOSX(folderPath, OSXPosixType.Folder);
            }

            return !Directory.Exists(folderPath);
        }

        /// <summary>
        /// Moves a file to trash on Windows utilizing FileSystem from VB
        /// </summary>
        /// <param name="filePath">The absolute path of the file to be deleted</param>
        private static async Task MoveFileToTrashWindows(string filePath)
        {
            await Task.Run(() => { FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin); });
        }

        /// <summary>
        /// Moves a folder to trash on Windows utilizing FileSystem from VB
        /// </summary>
        /// <param name="filePath">The absolute path of the file to be deleted</param>
        private static async Task MoveFolderToTrashWindows(string filePath)
        {
            await Task.Run(() => { FileSystem.DeleteDirectory(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin); });
        }

        /// <summary>
        /// Moves a file or folder to trash on OSX utilizing osascript
        ///  > inspired by StabilityMatrix' solution
        ///  > https://github.com/LykosAI/StabilityMatrix/blob/main/StabilityMatrix.Native.macOS/NativeRecycleBinProvider.cs
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private static async Task MoveToTrashOSX(string filePath, OSXPosixType type)
        {
            using (Process osaScriptProcess = new Process())
            {
                string script = $"tell application \\\"Finder\\\" to delete POSIX {type.ToString().ToLower()} \\\"{filePath}\\\"";

                osaScriptProcess.StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/osascript",
                    Arguments = $"-e \"{script}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                osaScriptProcess.Start();
                await osaScriptProcess.WaitForExitAsync().ConfigureAwait(false);

                if (osaScriptProcess.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Moving {filePath} to trash via osascript failed with exit code {osaScriptProcess.ExitCode}");
                }
            }
        }

        /// <summary>
        /// Moves a file or folder to trash on Linux, determined by current desktop environment
        ///  > compare electron implementation
        ///  > https://github.com/electron/electron/blob/6bf83b389bd752ae6e357cada84ed89e7beac0ba/shell/common/platform_util_linux.cc#L349
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static async Task MoveToTrashLinux(string filePath)
        {
            DesktopEnvironment desktopEnvironment = GetDesktopEnvironment();
            if (desktopEnvironment == DesktopEnvironment.Undefined) throw new InvalidOperationException($"Couldn't determine desktop environment");

            string[] args = [];
            string? command = null;

            if (desktopEnvironment == DesktopEnvironment.KDE5 ||
                desktopEnvironment == DesktopEnvironment.KDE4)
            {
                command = "kioclient5";
                args = ["move", filePath, "trash:/"];
            }
            else if (desktopEnvironment == DesktopEnvironment.KDE6)
            {
                command = "kioclient";
                args = ["move", filePath, "trash:/"];
            }
            else if (desktopEnvironment == DesktopEnvironment.Gnome ||
                    desktopEnvironment == DesktopEnvironment.Cinnamon)
            {
                command = "gio";
                args = ["trash", filePath];
            }
            else throw new InvalidOperationException($"Couldn't determine a way to move to trash");

            if (command != null && args != null)
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo()
                    {
                        FileName = command,
                        UseShellExecute = true,
                        CreateNoWindow = true
                    };

                    foreach (string arg in args)
                    {
                        process.StartInfo.ArgumentList.Add(arg);
                    }

                    process.Start();
                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
                        throw new InvalidOperationException($"Moving {filePath} to trash via {command} failed with exit code {process.ExitCode}");
                    }
                }
            }
        }

        /// <summary>
        /// Determines the current DE on linux
        /// see chromium source at https://chromium.googlesource.com/chromium/src/+/refs/heads/main/base/nix/xdg_util.cc
        /// </summary>
        /// <returns></returns>
        private static DesktopEnvironment GetDesktopEnvironment()
        {
            string[]? desktopEnvironmentValues = Environment.GetEnvironmentVariable(ENV_XDG_CURRENT_DESKTOP)
                ?.Split(':', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToUpper()).ToArray();

            if (desktopEnvironmentValues == null) return DesktopEnvironment.Undefined;

            foreach (string desktopEnvironment in desktopEnvironmentValues)
            {
                if (desktopEnvironment == DE_UNITY)
                {
                    // Gnome-Fallback set the environment variable to "Unity", so we have to determine it with the DESKTOP_SESSON environment var
                    string? desktopSessionEnv = Environment.GetEnvironmentVariable(ENV_DESKTOP_SESSION);
                    if (desktopSessionEnv != null && desktopSessionEnv.Contains("gnome-fallback"))
                    {
                        return DesktopEnvironment.Gnome;
                    }
                    return DesktopEnvironment.Unity;
                }
                else if (desktopEnvironment == DE_DEEPIN)
                {
                    return DesktopEnvironment.Deepin;
                }
                else if (desktopEnvironment == DE_GNOME)
                {
                    return DesktopEnvironment.Gnome;
                }
                else if (desktopEnvironment == DE_CINNAMON)
                {
                    return DesktopEnvironment.Cinnamon;
                }
                else if (desktopEnvironment == DE_KDE)
                {
                    string? kdeVersion = Environment.GetEnvironmentVariable(ENV_KDE_SESSION_VERSION);
                    if (kdeVersion == null) return DesktopEnvironment.KDE4;

                    if (kdeVersion == "5") return DesktopEnvironment.KDE5;
                    if (kdeVersion == "6") return DesktopEnvironment.KDE6;
                }
                else if (desktopEnvironment == DE_PANTHEON)
                {
                    return DesktopEnvironment.Pantheon;
                }
                else if (desktopEnvironment == DE_XFCE)
                {
                    return DesktopEnvironment.XFCE;
                }
                else if (desktopEnvironment == DE_UKUI)
                {
                    return DesktopEnvironment.UKUI;
                }
                else if (desktopEnvironment == DE_LXQT)
                {
                    return DesktopEnvironment.LXQt;
                }
            }

            return DesktopEnvironment.Undefined;
        }
    }
}

