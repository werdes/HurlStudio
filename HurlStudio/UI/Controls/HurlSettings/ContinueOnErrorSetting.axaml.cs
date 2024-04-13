using Avalonia.Controls;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class ContinueOnErrorSetting : BaseSettingControl<Collections.Settings.ContinueOnErrorSetting>
    {
        public ContinueOnErrorSetting(MainWindow mainWindow, ILogger<ContinueOnErrorSetting> logger,
            INotificationService notificationService) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}