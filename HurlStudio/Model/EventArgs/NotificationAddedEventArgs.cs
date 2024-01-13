using HurlStudio.Model.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class NotificationAddedEventArgs : System.EventArgs
    {
        private Notification _notification;

        public NotificationAddedEventArgs(Notification notification)
        {
            _notification = notification;
        }

        public Notification MyProperty
        {
            get => _notification;
        }
    }
}
