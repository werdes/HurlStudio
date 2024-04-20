using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class IpVersionSetting : BaseSettingControl<Collections.Settings.IpVersionSetting>
    {
        public IpVersionSetting(MainWindow mainWindow, ILogger<IpVersionSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}