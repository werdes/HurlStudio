using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Utilities;
using HurlStudio.Model.EventArgs;
using HurlStudio.Model.Notifications;

namespace HurlStudio.Services.Notifications
{
    public interface INotificationService
    {
        public event EventHandler<NotificationAddedEventArgs>? NotificationAdded;

        public void Notify(Notification notification);
        public void Notify(NotificationType notificationType, string title, string text);
        public void Notify(Exception exception);
        public void RemoveNotification(Notification notification);

    }
}
