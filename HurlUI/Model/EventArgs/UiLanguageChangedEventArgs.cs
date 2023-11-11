using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Model.EventArgs
{
    public class UiLanguageChangedEventArgs : System.EventArgs
    {
        private CultureInfo _culture;

        public CultureInfo Culture
        {
            get => this._culture;
            set => this._culture = value;
        }

        public UiLanguageChangedEventArgs(CultureInfo cultureInfo)
            => this._culture = cultureInfo;
    }
}
