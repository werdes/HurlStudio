using Microsoft.Extensions.Configuration;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Utility.Helpers
{
    public class AppEnvironmentHelper
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private static Lazy<AppEnvironmentHelper> _lazyInstance = new Lazy<AppEnvironmentHelper>(() => new AppEnvironmentHelper());

        public static AppEnvironmentHelper Instance => _lazyInstance.Value;

        private string _baseDir = null;
        private string _loggingPath = null;

        private bool _isInitialized;

        public bool IsInitialized
        {
            get => _isInitialized; 
            set => _isInitialized = value; 
        }

        /// <summary>
        /// Constructor for the helper
        ///  -> will automatically initialize the environment directories
        /// </summary>
        private AppEnvironmentHelper()
        {
            IsInitialized = this.InitializeRequiredDirectories();
        }

        /// <summary>
        /// Initializes the NLog logger configuration
        /// -> Avalonia previewer starts at the solution directory, which doesn't contain a logging configuration file
        /// </summary>
        public void InitializeLogging()
        {
            NLog.LogManager.Configuration = new NLog.Config.LoggingConfiguration();
            NLog.Targets.FileTarget loggingTarget = new NLog.Targets.FileTarget()
            {
                Name = "LogTarget",
                FileName = Path.Combine(_loggingPath, typeof(Program).FullName + ".log")
            };

#if DEBUG
            LoggingRule loggingRule = new LoggingRule("*", NLog.LogLevel.Debug, loggingTarget);
#else
            LoggingRule loggingRule = new LoggingRule("*", NLog.LogLevel.Info, loggingTarget);
#endif

            NLog.LogManager.Configuration.AddTarget(loggingTarget);
            NLog.LogManager.Configuration.AddRule(loggingRule);
        }


        /// <summary>
        /// Sets up the required directories
        /// </summary>
        public bool InitializeRequiredDirectories()
        {
            this._baseDir = App.Configuration.GetValue<string>("baseDirectory") ??
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HurlUI");

            this._loggingPath = Path.Combine(this._baseDir, "log");

            string[] requiredDirectories = new string[]
            {
                this._baseDir,
                this._loggingPath,
            };

            try
            {
                foreach (string directory in requiredDirectories)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                return true;
            }
            catch { }

            return false;
        }


        /// <summary>
        /// Returns the AppSettings json file location
        /// -> this helper method is not instance specific, because it is merely an Avalonia previewer helper
        /// -> Avalonia previewer starts at the solution directory, which doesn't contain the configuration file
        /// </summary>
        /// <returns> the AppSettings json file location</returns>
        public static string GetAppSettingsFile()
        {
            string[] possibleConfigurationFiles = new string[]
            {
                Path.Combine("HurlUI", "appsettings.json"),
                "appsettings.json"
            };

            return possibleConfigurationFiles.FirstOrDefault(x => File.Exists(x)) ?? "appsettings.json";
        }
    }
}
