using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class PathAsIsSetting : BaseSettingControl<Collections.Settings.PathAsIsSetting>
    {
        public PathAsIsSetting(MainWindow mainWindow, ILogger<PathAsIsSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}