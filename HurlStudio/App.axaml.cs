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
using HurlStudio.UI.Controls.HurlSettings;
using HurlStudio.Model.HurlSettings;
using HurlStudio.UI.ViewModels.Controls;
using System.Reflection;
using HurlStudio.Common.Extensions;
using NLog;

namespace HurlStudio
{
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
                this.RequestedThemeVariant = ThemeVariant.Dark;
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
                this.SetUiCulture();
                this.SetTheme();

                RegisterControls();
                RegisterViewModelHierarchy();

                if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
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

            if (Current != null && userSettings != null)
            {
                Current.RequestedThemeVariant = userSettings.Theme.GetThemeVariant();
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

            // Views
            controlBuilder.RegisterProviderAssociated<MainView, MainViewViewModel>(() => Services.GetRequiredService<MainView>());
            controlBuilder.RegisterProviderAssociated<LoadingView, LoadingViewViewModel>(() => Services.GetRequiredService<LoadingView>());
            controlBuilder.RegisterProviderAssociated<EditorView, EditorViewViewModel>(() => Services.GetRequiredService<EditorView>());

            // Controls
            controlBuilder.RegisterProviderAssociated<CollectionExplorerTool, CollectionExplorerToolViewModel>(() => Services.GetRequiredService<CollectionExplorerTool>());
            controlBuilder.RegisterProviderAssociated<FileSettingsTool, FileSettingsToolViewModel>(() => Services.GetRequiredService<FileSettingsTool>());
            controlBuilder.RegisterProviderAssociated<FileDocument, FileDocumentViewModel>(() => Services.GetRequiredService<FileDocument>());
            controlBuilder.RegisterProviderAssociated<WelcomeDocument, WelcomeDocumentViewModel>(() => Services.GetRequiredService<WelcomeDocument>());
            controlBuilder.RegisterProviderAssociated<RecentFile, FileHistoryEntry>(() => Services.GetRequiredService<RecentFile>());
            controlBuilder.RegisterProviderAssociated<NotificationCard, Notification>(() => Services.GetRequiredService<NotificationCard>());
            controlBuilder.RegisterProviderAssociated<SettingSection, HurlSettingSection>(() => Services.GetRequiredService<SettingSection>());

            controlBuilder.RegisterProviderAssociated<Collection, CollectionContainer>(() => Services.GetRequiredService<Collection>());
            controlBuilder.RegisterProviderAssociated<UI.Controls.CollectionExplorer.File, CollectionFile>(() => Services.GetRequiredService<UI.Controls.CollectionExplorer.File>());
            controlBuilder.RegisterProviderAssociated<Folder, CollectionFolder>(() => Services.GetRequiredService<Folder>());
            controlBuilder.RegisterProviderAssociated<ViewFrame, ViewFrameViewModel>(() => Services.GetRequiredService<ViewFrame>());

            // HurlSettings
            controlBuilder.RegisterProviderAssociated<SettingContainer, HurlSettingContainer>(() => Services.GetRequiredService<SettingContainer>());
            
            controlBuilder.RegisterProviderAssociated<AllowInsecureSetting, Collections.Settings.AllowInsecureSetting>(() => Services.GetRequiredService<AllowInsecureSetting>());
            controlBuilder.RegisterProviderAssociated<AwsSigV4Setting, Collections.Settings.AwsSigV4Setting>(() => Services.GetRequiredService<AwsSigV4Setting>());
            controlBuilder.RegisterProviderAssociated<BasicUserSetting, Collections.Settings.BasicUserSetting>(() => Services.GetRequiredService<BasicUserSetting>());
            controlBuilder.RegisterProviderAssociated<CaCertSetting, Collections.Settings.CaCertSetting>(() => Services.GetRequiredService<CaCertSetting>());
            controlBuilder.RegisterProviderAssociated<ClientCertificateSetting, Collections.Settings.ClientCertificateSetting>(() => Services.GetRequiredService<ClientCertificateSetting>());
            controlBuilder.RegisterProviderAssociated<ConnectToSetting, Collections.Settings.ConnectToSetting>(() => Services.GetRequiredService<ConnectToSetting>());
            controlBuilder.RegisterProviderAssociated<ContinueOnErrorSetting, Collections.Settings.ContinueOnErrorSetting>(() => Services.GetRequiredService<ContinueOnErrorSetting>());
            controlBuilder.RegisterProviderAssociated<CookieSetting, Collections.Settings.CookieSetting>(() => Services.GetRequiredService<CookieSetting>());
            controlBuilder.RegisterProviderAssociated<DelaySetting, Collections.Settings.DelaySetting>(() => Services.GetRequiredService<DelaySetting>());
            controlBuilder.RegisterProviderAssociated<FileRootSetting, Collections.Settings.FileRootSetting>(() => Services.GetRequiredService<FileRootSetting>());
            controlBuilder.RegisterProviderAssociated<HttpVersionSetting, Collections.Settings.HttpVersionSetting>(() => Services.GetRequiredService<HttpVersionSetting>());
            controlBuilder.RegisterProviderAssociated<IgnoreAssertsSetting, Collections.Settings.IgnoreAssertsSetting>(() => Services.GetRequiredService<IgnoreAssertsSetting>());
            controlBuilder.RegisterProviderAssociated<IpVersionSetting, Collections.Settings.IpVersionSetting>(() => Services.GetRequiredService<IpVersionSetting>());
            controlBuilder.RegisterProviderAssociated<ProxySetting, Collections.Settings.ProxySetting>(() => Services.GetRequiredService<ProxySetting>());
            controlBuilder.RegisterProviderAssociated<VariableSetting, Collections.Settings.VariableSetting>(() => Services.GetRequiredService<VariableSetting>());


            // Tools
            toolControlBuilder.RegisterProvider<CollectionExplorerToolViewModel>(() => Services.GetRequiredService<CollectionExplorerToolViewModel>());

            // Documents
            documentControlBuilder.RegisterProvider<FileDocumentViewModel>(() => Services.GetRequiredService<FileDocumentViewModel>());
            documentControlBuilder.RegisterProvider<WelcomeDocumentViewModel>(() => Services.GetRequiredService<WelcomeDocumentViewModel>());
        

        }

        /// <summary>
        ///  Registers the view models within each other
        /// </summary>
        private static void RegisterViewModelHierarchy()
        {
            MainViewViewModel mainViewViewModel = Services.GetRequiredService<MainViewViewModel>();
            mainViewViewModel.ViewFrameViewModel = Services.GetRequiredService<ViewFrameViewModel>();

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
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

                loggingBuilder.AddNLog(ConfigureLogging(baseDir));
            });
            services.AddDataProtection();
            
            
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
            services.AddSingleton<ViewFrameViewModel>();

            services.AddSingleton<ServiceManager<ViewModelBase>>(provider => new ServiceManager<ViewModelBase>()
                .Register(provider.GetRequiredService<MainViewViewModel>()));
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

            // Windows
            services.AddSingleton<MainWindow>();

            // Views
            services.AddSingleton<MainView>();
            services.AddSingleton<LoadingView>();
            services.AddSingleton<EditorView>();

            // Controls
            services.AddTransient<CollectionExplorerTool>();
            services.AddTransient<FileSettingsTool>();
            services.AddTransient<FileDocument>();
            services.AddTransient<WelcomeDocument>();
            services.AddTransient<RecentFile>();
            services.AddTransient<NotificationCard>();
            services.AddTransient<SettingSection>();
            services.AddTransient<ViewFrame>();

            // Collection Explorer components
            services.AddTransient<Collection>();
            services.AddTransient<UI.Controls.CollectionExplorer.File>();
            services.AddTransient<Folder>();

            // Hurl Settings
            services.AddTransient<SettingContainer>();

            services.AddTransient<AllowInsecureSetting>();
            services.AddTransient<AwsSigV4Setting>();
            services.AddTransient<BasicUserSetting>();
            services.AddTransient<CaCertSetting>();
            services.AddTransient<ClientCertificateSetting>();
            services.AddTransient<ConnectToSetting>();
            services.AddTransient<ContinueOnErrorSetting>();
            services.AddTransient<CookieSetting>();
            services.AddTransient<DelaySetting>();
            services.AddTransient<FileRootSetting>();
            services.AddTransient<HttpVersionSetting>();
            services.AddTransient<IgnoreAssertsSetting>();
            services.AddTransient<IpVersionSetting>();
            services.AddTransient<ProxySetting>();
            services.AddTransient<VariableSetting>();
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
        /// Initializes the NLog _log configuration
        /// -> Avalonia previewer starts at the solution directory, which doesn't contain a logging configuration file
        /// </summary>
        private static LoggingConfiguration ConfigureLogging(string baseDir)
        {
            LoggingConfiguration config = new LoggingConfiguration();

            // TODO: 
            // Keep for possible upcoming changes in NLog 5.3
            // see PR https://github.com/NLog/NLog/pull/5490
            // Possibly change from dedicated assembly Common.Extensions.Logging to type based exclusion via AddCallSiteHiddenClassType

            //config.LogFactory.Setup(setupBuilder =>
            //{
            //    setupBuilder.SetupLogFactory(logfactoryBuilder =>
            //    {
            //        logfactoryBuilder.AddCallSiteHiddenAssembly(Assembly.GetAssembly(typeof(Common.Extensions.Logging.ILoggerExtensions)));
            //        //logfactoryBuilder.AddCallSiteHiddenClassType(typeof(Common.Extensions.Logging.ILoggerExtensions));
            //    });
            //});

            NLog.LogManager.AddHiddenAssembly(Assembly.GetAssembly(typeof(Common.Logging.Extensions.ILoggerExtensions)));

            NLog.Targets.FileTarget loggingTarget = new NLog.Targets.FileTarget()
            {
                Name = "LogTarget",
                FileName = Path.Combine(baseDir, GlobalConstants.LOG_DIRECTORY_NAME, "hurlstudio.log"),
                Encoding = Encoding.UTF8,
                MaxArchiveFiles = 20,
                ArchiveOldFileOnStartup = true,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.DateAndSequence,
                ArchiveAboveSize = 10485760,
                ArchiveFileName = Path.Combine(baseDir, GlobalConstants.LOG_DIRECTORY_NAME, "hurlstudio.{#######}.a.log"),
                Layout = @"${longdate}| ${level:upperCase=true:padding=5}| ${callsite:includSourcePath=true}| ${message} ${exception:format=ToString}"
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
}
