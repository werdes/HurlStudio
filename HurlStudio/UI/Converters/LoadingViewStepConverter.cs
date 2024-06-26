﻿using Avalonia.Data.Converters;
using HurlStudio.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Converters
{
    public class LoadingViewStepConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value is not LoadingViewStep loadingViewStep) return string.Empty;
            
            switch (loadingViewStep)
            {
                case LoadingViewStep.LoadingCollections: return Localization.Localization.View_Loading_ActivityText_LoadingCollections;
                case LoadingViewStep.LoadingEnvironments: return Localization.Localization.View_Loading_ActivityText_LoadingEnvironments;
                case LoadingViewStep.OpeningPreviousSessionFiles: return Localization.Localization.View_Loading_ActivityText_OpeningPreviousSessionFiles;
            }

            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
