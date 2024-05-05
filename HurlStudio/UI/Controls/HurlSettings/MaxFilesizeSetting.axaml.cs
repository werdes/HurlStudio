using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class MaxFilesizeSetting : BaseSettingControl<Collections.Settings.MaxFilesizeSetting>
    {
        public MaxFilesizeSetting(MainWindow mainWindow, ILogger<MaxFilesizeSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}