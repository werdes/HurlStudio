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
                switch(dockable.GetType().Name)
                {
                    case nameof(CollectionDocumentViewModel): return Icon.Collection;
                    case nameof(EnvironmentDocumentViewModel): return Icon.Environment;
                    case nameof(FileDocumentViewModel): return Icon.File;
                    case nameof(FolderDocumentViewModel): return Icon.FolderClear;
                    case nameof(WelcomeDocumentViewModel): return Icon.Home;
                }
            }

            return Icon.Blank;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
