using Avalonia.Controls;
using Avalonia.Interactivity;
using HurlStudio.Model.Notifications;
using HurlStudio.Services.Notifications;

namespace HurlStudio.UI.Controls
{
    public partial class NotificationCard : ViewModelBasedControl<Notification>
    {
        private Notification? _notification;
        private INotificationService _notificationService;

        public NotificationCard(INotificationService notificationService)
        {
            _notificationService = notificationService;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(Notification viewModel)
        {
            _notification = viewModel;
            this.DataContext = viewModel;
        }

        protected void On_ButtonRemoveNotification_Click(object sender, RoutedEventArgs e)
        {
            if(_notification == null) return;
            
            _notificationService.RemoveNotification(_notification);
        }
    }
}
