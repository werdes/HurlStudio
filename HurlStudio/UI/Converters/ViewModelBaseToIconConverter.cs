using Avalonia.Data.Converters;
using HurlStudio.Model.Enums;
using HurlStudio.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class ViewModelBaseToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value != null && value is ViewModelBase viewModel)
            {
                if (viewModel is EditorViewViewModel) return Icon.Collection;
                if (viewModel is LoadingViewViewModel) return Icon.StatusBarIdle;

            }

            return Icon.Question32;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
