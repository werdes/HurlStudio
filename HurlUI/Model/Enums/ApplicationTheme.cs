using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Model.Enums
{
    public enum ApplicationTheme
    {
        Default = 0,
        Dark,
        Light
    }

    public static class ApplicationThemeExtensions
    {
        /// <summary>
        /// Returns the corresponding ThemeVariant record
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public static ThemeVariant GetThemeVariant(this ApplicationTheme theme)
        {
            switch (theme)
            {
                case ApplicationTheme.Dark: return ThemeVariant.Dark;
                case ApplicationTheme.Default: return ThemeVariant.Default;
                case ApplicationTheme.Light: return ThemeVariant.Light;
            }
            return ThemeVariant.Default;
        }
    }
}
