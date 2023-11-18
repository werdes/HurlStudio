using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Model.EventArgs
{
    public class ThemeChangedEventArgs : System.EventArgs
    {
        private CultureInfo _culture;

        public CultureInfo Culture
        {
            get => _culture;
            set => _culture = value;
        }

        public ThemeChangedEventArgs(CultureInfo cultureInfo)
            => _culture = cultureInfo;
    }
}
