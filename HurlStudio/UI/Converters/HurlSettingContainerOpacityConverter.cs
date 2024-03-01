using Avalonia.Data.Converters;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class HurlSettingContainerOpacityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value != null && value is HurlSettingContainer container)
            {
                if(!container.Enabled || container.Overwritten)
                {
                    return .5F;
                }
            }

            return 1F;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
