using Avalonia.Data.Converters;
using HurlStudio.Collections.Settings;
using HurlStudio.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class IHurlSettingToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IHurlSetting setting)
            {
                if (setting is ProxySetting) return Icon.Proxy;

            }
            return Icon.Setting;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
