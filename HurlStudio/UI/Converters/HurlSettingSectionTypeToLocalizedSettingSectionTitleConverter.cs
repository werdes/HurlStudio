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
    public class HurlSettingSectionTypeToLocalizedSettingSectionTitleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value != null && value is HurlSettingSectionType sectionType)
            {
                switch (sectionType)
                {
                    case HurlSettingSectionType.Environment: return Localization.Localization.Dock_Document_File_Settings_Environment_Title;
                    case HurlSettingSectionType.Collection: return Localization.Localization.Dock_Document_File_Settings_Collection_Title;
                    case HurlSettingSectionType.Folder: return Localization.Localization.Dock_Document_File_Settings_Folder_Title;
                    case HurlSettingSectionType.File: return Localization.Localization.Dock_Document_File_Settings_File_Title;
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
