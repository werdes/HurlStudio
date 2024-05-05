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
    public partial class UnixSocketSetting : BaseSettingControl<Collections.Settings.UnixSocketSetting>
    {
        public UnixSocketSetting(ILogger<UnixSocketSetting> logger, INotificationService notificationService,
            MainWindow mainWindow) : base(mainWindow, logger, notificationService)
        {
            this.InitializeComponent();
        }
    }
}