using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class AllowInsecureSetting : BaseSettingControl<Collections.Settings.AllowInsecureSetting>
    {
        public AllowInsecureSetting(MainWindow mainWindow, ILogger<AllowInsecureSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}