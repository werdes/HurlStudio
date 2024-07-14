using Avalonia.Data.Converters;
using Avalonia.Media;
using HurlStudio.Model.Enums;
using HurlStudio.Model.Notifications;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class NotificationTypeToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null && value is NotificationType notificationType)
            {
                return notificationType.GetIcon();
            }
            return Icon.Blank;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
