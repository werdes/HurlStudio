using Avalonia.Data.Converters;
using HurlStudio.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class IHurlSettingToLocalizedTitleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IHurlSetting setting)
            {
                string resourceName = $"Setting.{setting.GetType().Name}.Title";
                string? localizedText = Localization.Localization.ResourceManager.GetString(resourceName);

                if (localizedText != null)
                {
                    return localizedText;
                }
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
