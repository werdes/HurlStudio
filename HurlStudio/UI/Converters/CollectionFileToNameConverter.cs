using Avalonia.Data.Converters;
using HurlStudio.Model.HurlContainers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class CollectionFileToNameConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is HurlFileContainer)
            {
                var collectionFile = (HurlFileContainer)value;
                if(collectionFile != null)
                {
                    string title = collectionFile.File?.FileTitle ?? Path.GetFileName(collectionFile.Location);
                    return title;
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
