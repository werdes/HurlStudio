using Avalonia.Data.Converters;
using Dock.Model.Core;
using HurlStudio.Model.Enums;
using HurlStudio.UI.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class IDockableTypeToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is IDockable dockable)
            {
                if (dockable is CollectionDocumentViewModel) return Icon.Collection;
                if (dockable is FileDocumentViewModel) return Icon.File;
                if (dockable is FolderDocumentViewModel) return Icon.FolderClear;
                if (dockable is WelcomeDocumentViewModel) return Icon.Home;
            }

            return Icon.Blank;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
