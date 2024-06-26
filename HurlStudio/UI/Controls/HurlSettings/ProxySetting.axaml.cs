using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class ProxySetting : BaseSettingControl<Collections.Settings.ProxySetting>
    {
        public ProxySetting(MainWindow mainWindow, ILogger<ProxySetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}