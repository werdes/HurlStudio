using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HurlUI.UI.ViewModels;
using HurlUI.UI.Windows;
using HurlUI.Utility.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;

namespace HurlUI;

public partial class App : Application
{
    /// <summary>
    /// Configuration is provided as an application wide singleton
    /// Reasoning: Alternative would be DI, which is a mess with Avalonia without 
    ///            going the service locator way, i'm not a fan of either
    /// </summary>
    public static IConfiguration Configuration => _configuration.Value;
    private static Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(() =>
        new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile(AppEnvironmentHelper.GetAppSettingsFile(), true)
                                  .Build());

    /// <summary>
    /// Entrypoint after Avalonia initialization
    /// </summary>
    public override void Initialize()
    {
        UI.Localization.Localization.Culture = CultureInfo.InvariantCulture;

        if(AppEnvironmentHelper.Instance.IsInitialized)
        {
            AppEnvironmentHelper.Instance.InitializeLogging();
            AvaloniaXamlLoader.Load(this);
        }
    }

    /// <summary>
    /// Avalona Framework Initialization completed 
    /// - create the main window
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
