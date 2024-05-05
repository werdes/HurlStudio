using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HurlStudio.Collections.Settings;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Windows;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HurlStudio.UI.Controls.HurlSettings
{
    public partial class UserAgentSetting : BaseSettingControl<Collections.Settings.UserAgentSetting>
    {
        public UserAgentSetting(ILogger<UserAgentSetting> logger, INotificationService notificationService,
            MainWindow mainWindow) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}