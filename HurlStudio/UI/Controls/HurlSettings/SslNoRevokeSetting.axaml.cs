using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class SslNoRevokeSetting : BaseSettingControl<Collections.Settings.SslNoRevokeSetting>
    {
        public SslNoRevokeSetting(MainWindow mainWindow, ILogger<SslNoRevokeSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}