using Avalonia.Media;
using HurlStudio.Model.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.Notifications
{
    public enum NotificationType
    {
        None,
        Info,
        Warning,
        Error
    }

    public static class NotificationTypeExtensions
    {
        public static LogLevel GetLogLevel(this NotificationType type)
        {
            switch (type)
            {
                case NotificationType.None: return LogLevel.None;
                case NotificationType.Info: return LogLevel.Information;
                case NotificationType.Warning: return LogLevel.Warning;
                case NotificationType.Error: return LogLevel.Error;
                default: return LogLevel.Information;
            }
        }

        public static Brush GetBrush(this NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info: return new SolidColorBrush(Colors.DodgerBlue);
                case NotificationType.Warning: return new SolidColorBrush(Colors.Orange);
                case NotificationType.Error: return new SolidColorBrush(Colors.DarkRed);

                case NotificationType.None: 
                default: return new SolidColorBrush(Colors.Transparent);
            }
        }

        public static Icon GetIcon(this NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info: return Icon.NotificationInfo;
                case NotificationType.Warning: return Icon.NotificationWarning;
                case NotificationType.Error: return Icon.NotificationError;

                case NotificationType.None:
                default: return Icon.NotificationNone;
            }
        }
    }
}
