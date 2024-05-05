using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class RedirectionsSetting : BaseSettingControl<Collections.Settings.RedirectionsSetting>
    {
        public RedirectionsSetting(MainWindow mainWindow, ILogger<RedirectionsSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}