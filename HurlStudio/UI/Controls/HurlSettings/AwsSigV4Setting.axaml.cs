using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class AwsSigV4Setting : BaseSettingControl<Collections.Settings.AwsSigV4Setting>
    {
        public AwsSigV4Setting(MainWindow mainWindow, ILogger<AwsSigV4Setting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}