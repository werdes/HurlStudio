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
            if(value is IHurlSetting setting)
            {
                string propertyName = $"Setting_{setting.GetType().Name}_Title";
                PropertyInfo? localizationProperty = typeof(Localization.Localization).GetProperty(propertyName);
                if (localizationProperty != null)
                {
                    string? localizedText = (string?)localizationProperty.GetValue(null, null);
                    if (localizedText != null)
                    {
                        return localizedText;
                    }
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
