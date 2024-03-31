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
    public class ViewModelBaseToLocalizedLabelTextConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null && value is ViewModelBase viewModel)
            {
                if (viewModel is EditorViewViewModel) return Localization.Localization.View_Editor_Label;
                if (viewModel is LoadingViewViewModel) return Localization.Localization.View_Loading_Label;

            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
