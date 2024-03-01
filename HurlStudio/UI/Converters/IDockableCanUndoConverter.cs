using Avalonia.Data.Converters;
using HurlStudio.UI.Controls.Documents;
using HurlStudio.UI.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    internal class IDockableCanUndoConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value != null && value is FileDocumentViewModel file)
            {
                if (file.Document != null)
                {
                    return file.Document.UndoStack.CanUndo;
                }
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
