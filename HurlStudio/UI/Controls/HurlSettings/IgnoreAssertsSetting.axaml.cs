using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class IgnoreAssertsSetting : BaseSettingControl<Collections.Settings.IgnoreAssertsSetting>
    {
        public IgnoreAssertsSetting(MainWindow mainWindow, ILogger<IgnoreAssertsSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}