using System;
using Avalonia;

namespace HurlUI
{
    class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);


        /// <summary>
        /// Avalonia Initialization
        /// </summary>
        /// <returns></returns>
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
                                                                 .UsePlatformDetect()
                                                                 .WithInterFont()
                                                                 .LogToTrace();
    }
}