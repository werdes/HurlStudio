using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class ThemeChangedEventArgs : System.EventArgs
    {
        private ThemeVariant _theme;

        public ThemeVariant Culture
        {
            get => _theme;
            set => _theme = value;
        }

        public ThemeChangedEventArgs(ThemeVariant themeVariant)
            => _theme = themeVariant;
    }
}
