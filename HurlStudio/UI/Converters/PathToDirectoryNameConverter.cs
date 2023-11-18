using Avalonia.Data.Converters;
using HurlStudio.Model.CollectionContainer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class PathToDirectoryNameConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is string && !string.IsNullOrEmpty((string?)value))
            {
                string? path = value as string;
                if (path != null)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    return directoryInfo.Name;
                }
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
