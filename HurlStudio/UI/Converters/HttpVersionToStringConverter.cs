﻿using Avalonia.Data.Converters;
using HurlStudio.Common.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class HttpVersionToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is HttpVersion version)
            {
                return Localization.Localization.ResourceManager.GetString(version.GetLocalizationKey());
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
