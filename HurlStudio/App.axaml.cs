using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HurlStudio.Common;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI;
using HurlStudio.UI.Controls;
using HurlStudio.UI.Localization;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Views;
using HurlStudio.UI.Windows;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Fluent;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MsBox.Avalonia;
using HurlStudio.Collections.Utility;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using HurlStudio.Services.Editor;
using Dock.Model.Mvvm.Controls;
using HurlStudio.UI.Dock;
using Avalonia.Styling;
using HurlStudio.Model.Enums;
using HurlStudio.UI.Controls.Tools;
using HurlStudio.UI.Controls.Documents;
using HurlStudio.Services.UiState;
using HurlStudio.UI.ViewModels.Tools;
using HurlStudio.UI.ViewModels.Documents;
using HurlStudio.UI.Controls.CollectionExplorer;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.UiState;
using HurlStudio.Services.Notifications;
using HurlStudio.Model.Notifications;

namespace HurlStudio;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    public static IConfiguration? Config { get; private set; }

    /// <summary>
    /// Entrypoint after Avalonia initialization
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        if (Design.IsDesignMode)
        {
            RequestedThemeVariant = ThemeVariant.Dark;
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

        try
        {

            BuildServiceProvider();
            SetUiCulture();
            SetTheme();

            RegisterControls();
            RegisterViews();
            RegisterViewModelHierarchy();

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

            base.OnFrameworkInitializationCompleted();

#if DEBUG
            this.AttachDevTools();
#endif

        }
        catch (Exception ex)
        {
            IMsBox<ButtonResult> box = MessageBoxManager.GetMessageBoxStandard("Fatal error occured", ex.Message, ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            box.ShowWindowAsync();
        }
    }


    /// <summary>
    /// Sets the UI Culture for Localization
    /// </summary>
    private void SetUiCulture()
    {
        IUserSettingsService userSettingsService = Services.GetRequiredService<IUserSettingsService>();
        UserSettings? userSettings = userSettingsService.GetUserSettings(false);

        if (userSettings != null)
        {
            Localization.Culture = userSettings?.UiLanguage ?? CultureInfo.InvariantCulture;
        }
    }

    /// <summary>
    /// Sets the confirgured Theme
    /// </summary>
    private void SetTheme()
    {
        IUserSettingsService userSettingsService = Services.GetRequiredService<IUserSettingsService>();
        UserSettings? userSettings = userSettingsService.GetUserSettings(false);

        if (Application.Current != null && userSettings != null)
        {
            Application.Current.RequestedThemeVariant = userSettings.Theme.GetThemeVariant();
        }
    }

    /// <summary>
    /// Build the service provider
    /// </summary>
    private static void BuildServiceProvider()
    {
        IServiceCollection services = ConfigureServices();
        Services = services.BuildServiceProvider();
    }


    /// <summary>
    /// Registers user controls in the ControlBase service manager
    /// </summary>
    private static void RegisterControls()
    {
        ServiceManager<ViewModelBasedControl> controlBuilder = Services.GetRequiredService<ServiceManager<ViewModelBasedControl>>();
        ServiceManager<Tool> toolControlBuilder = Services.GetRequiredService<ServiceManager<Tool>>();
        ServiceManager<Document> documentControlBuilder = Services.GetRequiredService<ServiceManager<Document>>();

        controlBuilder.RegisterProviderAssociated<CollectionExplorerTool, CollectionExplorerToolViewModel>(() => Services.GetRequiredService<CollectionExplorerTool>());
        controlBuilder.RegisterProviderAssociated<FileSettingsTool, FileSettingsToolViewModel>(() => Services.GetRequiredService<FileSettingsTool>());
        controlBuilder.RegisterProviderAssociated<FileDocument, FileDocumentViewModel>(() => Services.GetRequiredService<FileDocument>());
        controlBuilder.RegisterProviderAssociated<WelcomeDocument, WelcomeDocumentViewModel>(() => Services.GetRequiredService<WelcomeDocument>());
        controlBuilder.RegisterProviderAssociated<RecentFile, FileHistoryEntry>(() => Services.GetRequiredService<RecentFile>());
        controlBuilder.RegisterProviderAssociated<NotificationCard, Notification>(() => Services.GetRequiredService<NotificationCard>());

        controlBuilder.RegisterProviderAssociated<UI.Controls.CollectionExplorer.Collection, CollectionContainer>(() => Services.GetRequiredService<UI.Controls.CollectionExplorer.Collection>());
        controlBuilder.RegisterProviderAssociated<UI.Controls.CollectionExplorer.File, CollectionFile>(() => Services.GetRequiredService<UI.Controls.CollectionExplorer.File>());
        controlBuilder.RegisterProviderAssociated<UI.Controls.CollectionExplorer.Folder, CollectionFolder>(() => Services.GetRequiredService<UI.Controls.CollectionExplorer.Folder>());

        toolControlBuilder.RegisterProvider<CollectionExplorerToolViewModel>(() => Services.GetRequiredService<CollectionExplorerToolViewModel>());
        toolControlBuilder.RegisterProvider<FileSettingsToolViewModel>(() => Services.GetRequiredService<FileSettingsToolViewModel>());

        documentControlBuilder.RegisterProvider<FileDocumentViewModel>(() => Services.GetRequiredService<FileDocumentViewModel>());
        documentControlBuilder.RegisterProvider<WelcomeDocumentViewModel>(() => Services.GetRequiredService<WelcomeDocumentViewModel>());
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
        string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GlobalConstants.APPLICATION_DIRECTORY_NAME);
        CreateRequiredDirectories(baseDir);

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);
            loggingBuilder.AddNLog(InitializeLogging(baseDir));
        });
        services.AddSingleton<LayoutFactory>();

        services.AddSingleton<IConfiguration>(Config);
        services.AddSingleton<IUserSettingsService, JsonUserSettingsService>();
        services.AddSingleton<IUiStateService, JsonUiStateService>();
        services.AddSingleton<IniSettingParser>();
        services.AddSingleton<ICollectionSerializer, IniCollectionSerializer>();
        services.AddSingleton<IEnvironmentSerializer, IniEnvironmentSerializer>();
        services.AddSingleton<IEditorService, EditorService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<ControlLocator>();

        services.AddSingleton<ICollectionService, CollectionService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();

        ConfigureDockControlViewmodels(services);
        ConfigureControls(services);
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
        services.AddSingleton<MainViewViewModel>();

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
    /// Configures the tool/document viewmodels
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigureDockControlViewmodels(IServiceCollection services)
    {
        // Viewmodel Builders
        services.AddSingleton<ServiceManager<Tool>>(provider => new ServiceManager<Tool>());
        services.AddSingleton<ServiceManager<Document>>(provider => new ServiceManager<Document>());

        // View models
        services.AddSingleton<CollectionExplorerToolViewModel>();
        services.AddTransient<FileSettingsToolViewModel>();
        services.AddTransient<FileDocumentViewModel>();
        services.AddTransient<WelcomeDocumentViewModel>();
    }

    /// <summary>
    /// Configures the controls
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigureControls(IServiceCollection services)
    {
        // Control Builder
        services.AddSingleton<ServiceManager<ViewModelBasedControl>>(provider => new ServiceManager<ViewModelBasedControl>());

        // Controls
        services.AddTransient<CollectionExplorerTool>();
        services.AddTransient<FileSettingsTool>();
        services.AddTransient<FileDocument>();
        services.AddTransient<WelcomeDocument>();
        services.AddTransient<RecentFile>();
        services.AddTransient<NotificationCard>();

        // Collection Explorer components
        services.AddTransient<UI.Controls.CollectionExplorer.Collection>();
        services.AddTransient<UI.Controls.CollectionExplorer.File>();
        services.AddTransient<UI.Controls.CollectionExplorer.Folder>();
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
            Path.Combine(baseDir, GlobalConstants.LOG_DIRECTORY_NAME),
            Path.Combine(baseDir, GlobalConstants.ENVIRONMENTS_DIRECTORY_NAME)
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
            FileName = Path.Combine(baseDir, GlobalConstants.LOG_DIRECTORY_NAME, "hurlstudio.log"),
            Encoding = Encoding.UTF8,
            MaxArchiveFiles = 3,
            ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Sequence,
            ArchiveAboveSize = 10485760,
            ArchiveFileName = Path.Combine(baseDir, GlobalConstants.LOG_DIRECTORY_NAME, "hurlstudio.{#######}.a"),
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
