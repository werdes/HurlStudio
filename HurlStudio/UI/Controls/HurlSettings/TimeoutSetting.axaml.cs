using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class TimeoutSetting : BaseSettingControl<Collections.Settings.TimeoutSetting>
    {
        public TimeoutSetting(MainWindow mainWindow, ILogger<TimeoutSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}