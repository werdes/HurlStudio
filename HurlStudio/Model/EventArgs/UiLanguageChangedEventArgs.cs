using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.EventArgs
{
    public class UiLanguageChangedEventArgs : System.EventArgs
    {
        private CultureInfo _culture;

        public CultureInfo Culture
        {
            get => _culture;
            set => _culture = value;
        }

        public UiLanguageChangedEventArgs(CultureInfo cultureInfo)
            => _culture = cultureInfo;
    }
}
