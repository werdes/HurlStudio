using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class HttpVersionSetting : BaseSettingControl<Collections.Settings.HttpVersionSetting>
    {
        public HttpVersionSetting(MainWindow mainWindow, ILogger<HttpVersionSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}