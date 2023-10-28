using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HurlUI.Common;
using HurlUI.UI;
using HurlUI.UI.Controls;
using HurlUI.UI.ViewModels;
using HurlUI.UI.Views;
using HurlUI.UI.Windows;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace HurlUI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    public static IConfiguration? Config { get; private set; }

    /// <summary>
    /// Entrypoint after Avalonia initialization
    /// </summary>
    public override void Initialize()
    {
        UI.Localization.Localization.Culture = CultureInfo.InvariantCulture;
        AvaloniaXamlLoader.Load(this);
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

        //if (!Design.IsDesignMode)
        //{
        BuildServiceProvider();


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindowViewModel? viewModel = Services?.GetRequiredService<MainWindowViewModel>();
            MainWindow? mainWindow = Services?.GetRequiredService<MainWindow>();
            if (mainWindow != null)
            {
                mainWindow.DataContext = viewModel;
                desktop.MainWindow = mainWindow;
            }
        }
        //}

        base.OnFrameworkInitializationCompleted();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    /// <summary>
    /// Build the service provider
    /// </summary>
    private static void BuildServiceProvider()
    {
        IServiceCollection services = ConfigureServices();
        Services = services.BuildServiceProvider();

        RegisterViews();
        RegisterViewModelHierarchy();
    }

    /// <summary>
    /// Registers the views in the viewBuilder service manager
    /// </summary>
    private static void RegisterViews()
    {
        ServiceManager<ViewBase> viewBuilder = Services.GetRequiredService<ServiceManager<ViewBase>>();
        viewBuilder.Register(Services.GetRequiredService<MainView>());
        viewBuilder.Register(Services.GetRequiredService<LoadingView>());
        viewBuilder.Register(Services.GetRequiredService<EditorView>());
    }

    /// <summary>
    ///  Registers the view models within each other
    /// </summary>
    private static void RegisterViewModelHierarchy()
    {
        MainViewViewModel mainViewViewModel = Services.GetRequiredService<MainViewViewModel>();
        mainViewViewModel.EditorView = Services.GetRequiredService<EditorViewViewModel>().SetRoot(mainViewViewModel) as EditorViewViewModel;
        mainViewViewModel.LoadingView = Services.GetRequiredService<LoadingViewViewModel>().SetRoot(mainViewViewModel) as LoadingViewViewModel;
    }

    /// <summary>
    /// Assembles the service collection
    /// </summary>
    /// <returns>The service collection</returns>
    private static IServiceCollection ConfigureServices()
    {
        ServiceCollection services = new ServiceCollection();

        Config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                           .AddJsonFile("appsettings.json", true)
                                           .Build();
        string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GlobalConstants.APPLICATION_FOLDER_NAME);
        CreateRequiredDirectories(baseDir);

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);
            loggingBuilder.AddNLog(InitializeLogging(baseDir));
        });
        services.AddSingleton<IConfiguration>(Config);
        ConfigureViews(services);
        ConfigureViewModels(services);


        return services;
    }

    /// <summary>
    /// Configures the view models
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<LoadingViewViewModel>();
        services.AddSingleton<EditorViewViewModel>();
        services.AddSingleton<MainViewViewModel>(); // provider => new MainViewViewModel(provider.GetRequiredService<MainWindowViewModel>())
        //{
        //    LoadingView = provider.GetRequiredService<LoadingViewViewModel>(),
        //    EditorView = provider.GetRequiredService<EditorViewViewModel>()
        //});

        services.AddSingleton<ServiceManager<ViewModelBase>>(provider => new ServiceManager<ViewModelBase>()
                                                                             .Register(provider.GetRequiredService<MainViewViewModel>()));
    }

    /// <summary>
    /// Configures the views
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigureViews(IServiceCollection services)
    {
        // View Builder
        services.AddSingleton<ServiceManager<ViewBase>>(provider => new ServiceManager<ViewBase>());

        // View Frame
        services.AddSingleton<ViewFrame>(provider => new ViewFrame(provider.GetRequiredService<MainViewViewModel>(), 
                                                                   provider.GetRequiredService<ServiceManager<ViewBase>>()));

        // Views
        services.AddSingleton<MainView>();
        services.AddSingleton<LoadingView>();
        services.AddSingleton<EditorView>();

        // Windows
        services.AddSingleton<MainWindow>();
    }

    /// <summary>
    /// Creates the required directories (under <paramref name="baseDir"/> folder)
    /// </summary>
    /// <param name="baseDir">The base directory (most likely the ApplicationData special folder)</param>
    private static void CreateRequiredDirectories(string baseDir)
    {
        string[] requiredDirectories = new string[]
        {
            baseDir,
            Path.Combine(baseDir, "log"),
        };

        foreach (string directory in requiredDirectories)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }

    /// <summary>
    /// Initializes the NLog logger configuration
    /// -> Avalonia previewer starts at the solution directory, which doesn't contain a logging configuration file
    /// </summary>
    private static LoggingConfiguration InitializeLogging(string baseDir)
    {
        LoggingConfiguration config = new LoggingConfiguration();
        NLog.Targets.FileTarget loggingTarget = new NLog.Targets.FileTarget()
        {
            Name = "LogTarget",
            FileName = Path.Combine(baseDir, "log", "hurlui.log"),
            Encoding = Encoding.UTF8,
            MaxArchiveFiles = 3,
            ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Sequence,
            ArchiveAboveSize = 10485760,
            ArchiveFileName = Path.Combine(baseDir, "log", "hurlui.{#######}.a"),
            Layout = @"${longdate}| ${level:upperCase=true:padding=5}| ${callsite:includSourcePath=true:padding=100}| ${message} ${exception:format=ToString}"
            //Layout = @"${longdate}|${level}|${message} |${all-event-properties} ${exception:format=tostring}"
        };

#if DEBUG
        LoggingRule loggingRule = new LoggingRule("*", loggingTarget);
        loggingRule.EnableLoggingForLevels(NLog.LogLevel.Trace, NLog.LogLevel.Fatal);
#else
        LoggingRule loggingRule = new LoggingRule("*", NLog.LogLevel.Info, loggingTarget);
        loggingRule.EnableLoggingForLevels(NLog.LogLevel.Info, NLog.LogLevel.Fatal);
#endif

        config.AddRule(loggingRule);
        config.AddTarget(loggingTarget);

        return config;
    }
}
