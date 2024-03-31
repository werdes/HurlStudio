using Avalonia.Data.Converters;
using HurlStudio.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class HurlSettingSectionTypeToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value != null && value is HurlSettingSectionType sectionType)
            {
                switch (sectionType)
                {
                    case HurlSettingSectionType.Environment: return Icon.Environment;
                    case HurlSettingSectionType.Collection: return Icon.Collection;
                    case HurlSettingSectionType.Folder: return Icon.FolderClear;
                    case HurlSettingSectionType.File: return Icon.File;
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
