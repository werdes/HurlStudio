using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HurlStudio.Common;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Controls;
using HurlStudio.UI.Localization;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Views;
using HurlStudio.UI.Windows;
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
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.UiState;
using HurlStudio.Services.Notifications;
using HurlStudio.Model.Notifications;
using HurlStudio.UI.Controls.HurlSettings;
using HurlStudio.Model.HurlSettings;
using HurlStudio.UI.ViewModels.Controls;
using System.Reflection;
using HurlStudio.Common.Extensions;
using NLog;
using HurlStudio.Utility;
using HurlStudio.UI.ViewModels.Windows;
using ActiproSoftware.Properties.Shared;
using HurlStudio.UI;
using HurlStudio.Services.HurlFileTemplates;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.UI.MessageBox;

namespace HurlStudio
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        public static IConfiguration? Config { get; private set; }

        /// <summary>
        /// Entrypoint after Avalonia initialization
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            this.SetResourceOverrides();

            if (Design.IsDesignMode)
            {
                this.RequestedThemeVariant = ThemeVariant.Dark;
            }
        }

        /// <summary>
        /// Set static resources before first UI reference
        /// </summary>
        private void SetResourceOverrides()
        {
            SR.SetCustomString(SRName.UICaptionButtonCloseText, Localization.Common_Window_Close);
            SR.SetCustomString(SRName.UICaptionButtonEnterFullScreenText, Localization.Common_Window_EnterFullScreen);
            SR.SetCustomString(SRName.UICaptionButtonExitFullScreenText, Localization.Common_Window_ExitFullScreen);
            SR.SetCustomString(SRName.UICaptionButtonMaximizeText, Localization.Common_Window_Maximize);
            SR.SetCustomString(SRName.UICaptionButtonMinimizeText, Localization.Common_Window_Minimize);
            SR.SetCustomString(SRName.UICaptionButtonRestoreText, Localization.Common_Window_Restore);
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
                    MainWindow? mainWindow = Services?.GetRequiredService<MainWindow>();
                    if (mainWindow != null)
                    {
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
                
#pragma warning disable CS4014
                MessageBox.ShowError(ex.Message, "Fatal error occured");
#pragma warning restore CS4014
            }
        }


        /// <summary>
        /// Sets the UI Culture for Localization
        /// </summary>
        private void SetUiCulture()
        {
            if (Services == null) throw new ArgumentNullException(nameof(Services));

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
            if (Services == null) throw new ArgumentNullException(nameof(Services));

            IUserSettingsService userSettingsService = Services.GetRequiredService<IUserSettingsService>();
            UserSettings? userSettings = userSettingsService.GetUserSettings(false);

            if (Current != null && userSettings != null)
            {
                Current.RequestedThemeVariant = userSettings.Theme.GetThemeVariant();
                MessageBox.ThemeVariant = Current.RequestedThemeVariant;
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
            if (Services == null) throw new ArgumentNullException(nameof(Services));

            ServiceManager<ViewModelBasedControl> controlBuilder = Services.GetRequiredService<ServiceManager<ViewModelBasedControl>>();
            ServiceManager<UI.Windows.WindowBase> windowBuilder = Services.GetRequiredService<ServiceManager<UI.Windows.WindowBase>>();
            ServiceManager<Tool> toolControlBuilder = Services.GetRequiredService<ServiceManager<Tool>>();
            ServiceManager<Document> documentControlBuilder = Services.GetRequiredService<ServiceManager<Document>>();

            // Windows
            windowBuilder.RegisterProvider<MainWindow>(() => Services.GetRequiredService<MainWindow>());
            windowBuilder.RegisterProvider<AddSettingWindow>(() => Services.GetRequiredService<AddSettingWindow>());
            windowBuilder.RegisterProvider<AddFileWindow>(() => Services.GetRequiredService<AddFileWindow>());
            windowBuilder.RegisterProvider<EditTemplateWindow>(() => Services.GetRequiredService<EditTemplateWindow>());
            windowBuilder.RegisterProvider<AddCollectionWindow>(() => Services.GetRequiredService<AddCollectionWindow>());
            windowBuilder.RegisterProvider<AddEnvironmentWindow>(() => Services.GetRequiredService<AddEnvironmentWindow>());

            // Views
            controlBuilder.RegisterProviderAssociated<MainView, MainViewViewModel>(() => Services.GetRequiredService<MainView>());
            controlBuilder.RegisterProviderAssociated<LoadingView, LoadingViewViewModel>(() => Services.GetRequiredService<LoadingView>());
            controlBuilder.RegisterProviderAssociated<EditorView, EditorViewViewModel>(() => Services.GetRequiredService<EditorView>());
            controlBuilder.RegisterProviderAssociated<AddSettingView, AddSettingViewViewModel>(() => Services.GetRequiredService<AddSettingView>());
            controlBuilder.RegisterProviderAssociated<AddFileView, AddFileViewViewModel>(() => Services.GetRequiredService<AddFileView>());
            controlBuilder.RegisterProviderAssociated<EditTemplateView, EditTemplateViewViewModel>(() => Services.GetRequiredService<EditTemplateView>());
            controlBuilder.RegisterProviderAssociated<AddCollectionView, AddCollectionViewViewModel>(() => Services.GetRequiredService<AddCollectionView>());
            controlBuilder.RegisterProviderAssociated<AddEnvironmentView, AddEnvironmentViewViewModel>(() => Services.GetRequiredService<AddEnvironmentView>());

            // Dock controls
            controlBuilder.RegisterProviderAssociated<CollectionExplorerTool, CollectionExplorerToolViewModel>(() => Services.GetRequiredService<CollectionExplorerTool>());
            controlBuilder.RegisterProviderAssociated<EnvironmentExplorerTool, EnvironmentExplorerToolViewModel>(() => Services.GetRequiredService<EnvironmentExplorerTool>());
            controlBuilder.RegisterProviderAssociated<FileDocument, FileDocumentViewModel>(() => Services.GetRequiredService<FileDocument>());
            controlBuilder.RegisterProviderAssociated<WelcomeDocument, WelcomeDocumentViewModel>(() => Services.GetRequiredService<WelcomeDocument>());
            controlBuilder.RegisterProviderAssociated<FolderDocument, FolderDocumentViewModel>(() => Services.GetRequiredService<FolderDocument>());
            controlBuilder.RegisterProviderAssociated<CollectionDocument, CollectionDocumentViewModel>(() => Services.GetRequiredService<CollectionDocument>());
            controlBuilder.RegisterProviderAssociated<EnvironmentDocument, EnvironmentDocumentViewModel>(() => Services.GetRequiredService<EnvironmentDocument>());


            // Controls
            controlBuilder.RegisterProviderAssociated<RecentFile, FileHistoryEntry>(() => Services.GetRequiredService<RecentFile>());
            controlBuilder.RegisterProviderAssociated<NotificationCard, Notification>(() => Services.GetRequiredService<NotificationCard>());
            controlBuilder.RegisterProviderAssociated<SettingSection, HurlSettingSection>(() => Services.GetRequiredService<SettingSection>());

            controlBuilder.RegisterProviderAssociated<Collection, HurlCollectionContainer>(() => Services.GetRequiredService<Collection>());
            controlBuilder.RegisterProviderAssociated<UI.Controls.CollectionExplorer.File, HurlFileContainer>(() => Services.GetRequiredService<UI.Controls.CollectionExplorer.File>());
            controlBuilder.RegisterProviderAssociated<Folder, HurlFolderContainer>(() => Services.GetRequiredService<Folder>());
            controlBuilder.RegisterProviderAssociated<UI.Controls.EnvironmentExplorer.Environment, HurlEnvironmentContainer>(() => Services.GetRequiredService<UI.Controls.EnvironmentExplorer.Environment>());
            controlBuilder.RegisterProviderAssociated<ViewFrame, ViewFrameViewModel>(() => Services.GetRequiredService<ViewFrame>());
            controlBuilder.RegisterProviderAssociated<AdditionalLocation, Collections.Model.Containers.AdditionalLocation>(() => Services.GetRequiredService<AdditionalLocation>());
            controlBuilder.RegisterProviderAssociated<SettingContainer, HurlSettingContainer>(() => Services.GetRequiredService<SettingContainer>());
            controlBuilder.RegisterProviderAssociated<SettingTypeContainer, HurlSettingTypeContainer>(() => Services.GetRequiredService<SettingTypeContainer>());
            controlBuilder.RegisterProviderAssociated<HurlFileTemplateListItem, HurlFileTemplateContainer>(() => Services.GetRequiredService<HurlFileTemplateListItem>());

            // HurlSettings

            controlBuilder.RegisterProviderAssociated<AllowInsecureSetting, Collections.Settings.AllowInsecureSetting>(() => Services.GetRequiredService<AllowInsecureSetting>());
            controlBuilder.RegisterProviderAssociated<AwsSigV4Setting, Collections.Settings.AwsSigV4Setting>(() => Services.GetRequiredService<AwsSigV4Setting>());
            controlBuilder.RegisterProviderAssociated<BasicUserSetting, Collections.Settings.BasicUserSetting>(() => Services.GetRequiredService<BasicUserSetting>());
            controlBuilder.RegisterProviderAssociated<CaCertSetting, Collections.Settings.CaCertSetting>(() => Services.GetRequiredService<CaCertSetting>());
            controlBuilder.RegisterProviderAssociated<ClientCertificateSetting, Collections.Settings.ClientCertificateSetting>(() => Services.GetRequiredService<ClientCertificateSetting>());
            controlBuilder.RegisterProviderAssociated<ConnectToSetting, Collections.Settings.ConnectToSetting>(() => Services.GetRequiredService<ConnectToSetting>());
            controlBuilder.RegisterProviderAssociated<ContinueOnErrorSetting, Collections.Settings.ContinueOnErrorSetting>(() => Services.GetRequiredService<ContinueOnErrorSetting>());
            controlBuilder.RegisterProviderAssociated<CookieSetting, Collections.Settings.CookieSetting>(() => Services.GetRequiredService<CookieSetting>());
            controlBuilder.RegisterProviderAssociated<CustomArgumentsSetting, Collections.Settings.CustomArgumentsSetting>(() => Services.GetRequiredService<CustomArgumentsSetting>());
            controlBuilder.RegisterProviderAssociated<DelaySetting, Collections.Settings.DelaySetting>(() => Services.GetRequiredService<DelaySetting>());
            controlBuilder.RegisterProviderAssociated<FileRootSetting, Collections.Settings.FileRootSetting>(() => Services.GetRequiredService<FileRootSetting>());
            controlBuilder.RegisterProviderAssociated<HttpVersionSetting, Collections.Settings.HttpVersionSetting>(() => Services.GetRequiredService<HttpVersionSetting>());
            controlBuilder.RegisterProviderAssociated<IgnoreAssertsSetting, Collections.Settings.IgnoreAssertsSetting>(() => Services.GetRequiredService<IgnoreAssertsSetting>());
            controlBuilder.RegisterProviderAssociated<IpVersionSetting, Collections.Settings.IpVersionSetting>(() => Services.GetRequiredService<IpVersionSetting>());
            controlBuilder.RegisterProviderAssociated<MaxFilesizeSetting, Collections.Settings.MaxFilesizeSetting>(() => Services.GetRequiredService<MaxFilesizeSetting>());
            controlBuilder.RegisterProviderAssociated<NetrcSetting, Collections.Settings.NetrcSetting>(() => Services.GetRequiredService<NetrcSetting>());
            controlBuilder.RegisterProviderAssociated<NoProxySetting, Collections.Settings.NoProxySetting>(() => Services.GetRequiredService<NoProxySetting>());
            controlBuilder.RegisterProviderAssociated<PathAsIsSetting, Collections.Settings.PathAsIsSetting>(() => Services.GetRequiredService<PathAsIsSetting>());
            controlBuilder.RegisterProviderAssociated<ProxySetting, Collections.Settings.ProxySetting>(() => Services.GetRequiredService<ProxySetting>());
            controlBuilder.RegisterProviderAssociated<RedirectionsSetting, Collections.Settings.RedirectionsSetting>(() => Services.GetRequiredService<RedirectionsSetting>());
            controlBuilder.RegisterProviderAssociated<ResolveSetting, Collections.Settings.ResolveSetting>(() => Services.GetRequiredService<ResolveSetting>());
            controlBuilder.RegisterProviderAssociated<RetrySetting, Collections.Settings.RetrySetting>(() => Services.GetRequiredService<RetrySetting>());
            controlBuilder.RegisterProviderAssociated<SslNoRevokeSetting, Collections.Settings.SslNoRevokeSetting>(() => Services.GetRequiredService<SslNoRevokeSetting>());
            controlBuilder.RegisterProviderAssociated<TimeoutSetting, Collections.Settings.TimeoutSetting>(() => Services.GetRequiredService<TimeoutSetting>());
            controlBuilder.RegisterProviderAssociated<ToEntrySetting, Collections.Settings.ToEntrySetting>(() => Services.GetRequiredService<ToEntrySetting>());
            controlBuilder.RegisterProviderAssociated<UnixSocketSetting, Collections.Settings.UnixSocketSetting>(() => Services.GetRequiredService<UnixSocketSetting>());
            controlBuilder.RegisterProviderAssociated<UserAgentSetting, Collections.Settings.UserAgentSetting>(() => Services.GetRequiredService<UserAgentSetting>());
            controlBuilder.RegisterProviderAssociated<VariableSetting, Collections.Settings.VariableSetting>(() => Services.GetRequiredService<VariableSetting>());
            controlBuilder.RegisterProviderAssociated<VariablesFileSetting, Collections.Settings.VariablesFileSetting>(() => Services.GetRequiredService<VariablesFileSetting>());


            // Tools
            toolControlBuilder.RegisterProvider<CollectionExplorerToolViewModel>(() => Services.GetRequiredService<CollectionExplorerToolViewModel>());
            toolControlBuilder.RegisterProvider<EnvironmentExplorerToolViewModel>(() => Services.GetRequiredService<EnvironmentExplorerToolViewModel>());

            // Documents
            documentControlBuilder.RegisterProvider<FileDocumentViewModel>(() => Services.GetRequiredService<FileDocumentViewModel>());
            documentControlBuilder.RegisterProvider<WelcomeDocumentViewModel>(() => Services.GetRequiredService<WelcomeDocumentViewModel>());
            documentControlBuilder.RegisterProvider<FolderDocumentViewModel>(() => Services.GetRequiredService<FolderDocumentViewModel>());
            documentControlBuilder.RegisterProvider<CollectionDocumentViewModel>(() => Services.GetRequiredService<CollectionDocumentViewModel>());
            documentControlBuilder.RegisterProvider<EnvironmentDocumentViewModel>(() => Services.GetRequiredService<EnvironmentDocumentViewModel>());
        }

        /// <summary>
        ///  Registers the view models within each other
        /// </summary>
        private static void RegisterViewModelHierarchy()
        {
            if (Services == null) throw new ArgumentNullException(nameof(Services));

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
            services.AddSingleton<IHurlFileTemplateService, JsonListHurlFileTemplateService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddTransient<ControlLocator>();

            services.AddSingleton<ICollectionService, CollectionService>();
            services.AddSingleton<IEnvironmentService, EnvironmentService>();

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

            // Window view models
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<AddSettingWindowViewModel>();
            services.AddTransient<AddFileWindowViewModel>();
            services.AddTransient<EditTemplateWindowViewModel>();
            services.AddTransient<AddCollectionWindowViewModel>();
            services.AddTransient<AddEnvironmentWindowViewModel>();

            // View view models
            services.AddSingleton<LoadingViewViewModel>();
            services.AddSingleton<EditorViewViewModel>();
            services.AddSingleton<MainViewViewModel>();
            services.AddSingleton<ViewFrameViewModel>();
            services.AddTransient<AddSettingViewViewModel>();
            services.AddTransient<AddFileViewViewModel>();
            services.AddTransient<EditTemplateViewViewModel>();
            services.AddTransient<AddCollectionViewViewModel>();
            services.AddTransient<AddEnvironmentViewViewModel>();

            // Dock view models
            services.AddSingleton<CollectionExplorerToolViewModel>();
            services.AddSingleton<EnvironmentExplorerToolViewModel>();
            services.AddTransient<FileDocumentViewModel>();
            services.AddTransient<FolderDocumentViewModel>();
            services.AddTransient<CollectionDocumentViewModel>();
            services.AddTransient<EnvironmentDocumentViewModel>();
            services.AddTransient<WelcomeDocumentViewModel>();


            // Viewmodel Builders
            services.AddSingleton<ServiceManager<Tool>>(provider => new ServiceManager<Tool>());
            services.AddSingleton<ServiceManager<Document>>(provider => new ServiceManager<Document>());
            services.AddSingleton<ServiceManager<ViewModelBase>>(provider => new ServiceManager<ViewModelBase>()
                .Register(provider.GetRequiredService<MainViewViewModel>()));
        }

        /// <summary>
        /// Configures the controls
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureControls(IServiceCollection services)
        {
            // Control Builders
            services.AddSingleton<ServiceManager<UI.Windows.WindowBase>>(provider => new ServiceManager<UI.Windows.WindowBase>());
            services.AddSingleton<ServiceManager<ViewModelBasedControl>>(provider => new ServiceManager<ViewModelBasedControl>());

            // Windows
            services.AddSingleton<MainWindow>();
            services.AddTransient<AddSettingWindow>();
            services.AddTransient<AddFileWindow>();
            services.AddTransient<EditTemplateWindow>();
            services.AddTransient<AddCollectionWindow>();
            services.AddTransient<AddEnvironmentWindow>();

            // Views
            services.AddSingleton<MainView>();
            services.AddSingleton<LoadingView>();
            services.AddSingleton<EditorView>();
            services.AddTransient<AddSettingView>();
            services.AddTransient<AddFileView>();
            services.AddTransient<EditTemplateView>();
            services.AddTransient<AddCollectionView>();
            services.AddTransient<AddEnvironmentView>();

            // Dock controls
            services.AddTransient<CollectionExplorerTool>();
            services.AddTransient<EnvironmentExplorerTool>();
            services.AddTransient<FileDocument>();
            services.AddTransient<FolderDocument>();
            services.AddTransient<CollectionDocument>();
            services.AddTransient<EnvironmentDocument>();
            

            // Controls
            services.AddTransient<WelcomeDocument>();
            services.AddTransient<RecentFile>();
            services.AddTransient<NotificationCard>();
            services.AddTransient<SettingSection>();
            services.AddTransient<ViewFrame>();
            services.AddTransient<AdditionalLocation>();
            services.AddTransient<SettingContainer>();
            services.AddTransient<SettingTypeContainer>();
            services.AddTransient<HurlFileTemplateListItem>();
            services.AddTransient<Collection>();
            services.AddTransient<UI.Controls.CollectionExplorer.File>();
            services.AddTransient<Folder>();
            services.AddTransient<UI.Controls.EnvironmentExplorer.Environment>();

            // Hurl Settings
            services.AddTransient<AllowInsecureSetting>();
            services.AddTransient<AwsSigV4Setting>();
            services.AddTransient<BasicUserSetting>();
            services.AddTransient<CaCertSetting>();
            services.AddTransient<ClientCertificateSetting>();
            services.AddTransient<ConnectToSetting>();
            services.AddTransient<ContinueOnErrorSetting>();
            services.AddTransient<CookieSetting>();
            services.AddTransient<CustomArgumentsSetting>();
            services.AddTransient<DelaySetting>();
            services.AddTransient<FileRootSetting>();
            services.AddTransient<HttpVersionSetting>();
            services.AddTransient<IgnoreAssertsSetting>();
            services.AddTransient<IpVersionSetting>();
            services.AddTransient<MaxFilesizeSetting>();
            services.AddTransient<NetrcSetting>();
            services.AddTransient<NoProxySetting>();
            services.AddTransient<PathAsIsSetting>();
            services.AddTransient<ProxySetting>();
            services.AddTransient<RedirectionsSetting>();
            services.AddTransient<ResolveSetting>();
            services.AddTransient<RetrySetting>();
            services.AddTransient<SslNoRevokeSetting>();
            services.AddTransient<TimeoutSetting>();
            services.AddTransient<ToEntrySetting>();
            services.AddTransient<UnixSocketSetting>();
            services.AddTransient<UserAgentSetting>();
            services.AddTransient<VariableSetting>();
            services.AddTransient<VariablesFileSetting>();

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

            config.LogFactory.Setup(setupBuilder =>
            {
                setupBuilder.SetupLogFactory(logfactoryBuilder =>
                {
                    logfactoryBuilder.AddCallSiteHiddenClassType(typeof(Common.Extensions.ILoggerExtensions));
                });
            });

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
            NLog.LogLevel releaseLogLevel = Config?.GetValue<NLog.LogLevel>("logLevel") ?? NLog.LogLevel.Trace;
            LoggingRule loggingRule = new LoggingRule("*", releaseLogLevel, loggingTarget);
            loggingRule.EnableLoggingForLevels(releaseLogLevel, NLog.LogLevel.Fatal);
#endif

            config.AddRule(loggingRule);
            config.AddTarget(loggingTarget);

            return config;
        }
    }
}
