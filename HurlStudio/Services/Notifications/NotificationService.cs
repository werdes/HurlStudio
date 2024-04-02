using HurlStudio.Model.EventArgs;
using HurlStudio.Model.Notifications;
using HurlStudio.UI.Localization;
using HurlStudio.UI.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        public event EventHandler<NotificationAddedEventArgs>? NotificationAdded;

        private ILogger _log;
        private MainViewViewModel _mainViewModel;

        public NotificationService(ILogger<NotificationService> logger, MainViewViewModel mainViewViewModel)
        {
            _log = logger;
            _mainViewModel = mainViewViewModel;
        }

        /// <summary>
        /// Supplies a notification to the main view viewmodel notification list
        /// </summary>
        /// <param name="notification"></param>
        public void Notify(Notification notification)
        {
            _log.Log(notification.Type.GetLogLevel(), notification.GetLogMessage());

            _mainViewModel.Notifications.Add(notification);

            this.NotificationAdded?.Invoke(this, new NotificationAddedEventArgs(notification));
        }

        public void Notify(NotificationType notificationType, string title, string text) => this.Notify(new Notification(notificationType, title, text));

        public void Notify(Exception exception) => this.Notify(new Notification(NotificationType.Error, Localization.Common_Exception_Title, exception.Message));

        /// <summary>
        /// Removes a notification from the main view viewmodels notification list
        /// </summary>
        /// <param name="notification"></param>
        public void RemoveNotification(Notification notification)
        {
            _log.LogDebug($"Removing notification [{notification}]");
            _mainViewModel.Notifications.Remove(notification);
        }
    }
}
