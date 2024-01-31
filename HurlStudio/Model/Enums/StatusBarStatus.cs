using Avalonia.Media;
using HurlStudio.UI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HurlStudio.Model.Enums
{
    public enum StatusBarStatus
    {
        Idle,
        OpeningFile
    }

    public static class StatusBarStatusExtensions
    {
        public static string GetLocalizedText(this StatusBarStatus status)
        {
            string propertyName = $"View_Main_StatusBar_Status_{status}_Text";
            PropertyInfo? localizationProperty = typeof(Localization).GetProperty(propertyName);
            if (localizationProperty != null)
            {
                string? localizedText = (string?)localizationProperty.GetValue(null, null);
                if (localizedText != null)
                {
                    return localizedText;
                }
            }

            return string.Empty;
        }

        public static Icon GetIcon(this StatusBarStatus status)
        {
            switch (status)
            {
                case StatusBarStatus.OpeningFile:
                    return Icon.OpenNeutral;
                case StatusBarStatus.Idle:
                default:
                    return Icon.StatusBarIdle;
            }
        }

        public static Brush GetBackgroundBrush(this StatusBarStatus status)
        {
            switch (status) { 
                case StatusBarStatus.Idle:
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
