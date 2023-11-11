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
            get => this._theme;
            set => this._theme = value;
        }

        public ThemeChangedEventArgs(ThemeVariant themeVariant)
            => this._theme = themeVariant;
    }
}
