using Avalonia.Styling;
using HurlStudio.Model.Enums;
using HurlStudio.Model.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HurlStudio.Model.UserSettings
{
    public class UserSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<UiLanguageChangedEventArgs>? UiLanguageChanged;
        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private CultureInfo _uiLanguage;
        private ObservableCollection<string>? _collectionFiles;
        private ApplicationTheme _theme;

        public UserSettings(CultureInfo defaultUiLanguage, ApplicationTheme defaultTheme)
        {
            _uiLanguage = defaultUiLanguage;
            _collectionFiles = new ObservableCollection<string>();
            _theme = defaultTheme;
        }

        /// <summary>
        /// Parameterless constructor for deserialization
        /// </summary>
        public UserSettings()
        {
            _uiLanguage = CultureInfo.InvariantCulture;
            _collectionFiles = new ObservableCollection<string>();
        }

        /// <summary>
        /// User selected theme variant
        /// Ignore this due to System.Text.Json serializer serializing the ThemeVariant record
        /// </summary>
        //[JsonIgnore]
        [JsonPropertyName("theme")]
        public ApplicationTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                Notify();
            }
        }

        //public string ThemeString
        //{
        //    get => _theme.Key.ToString() ?? "Dark";
        //    set => _theme = new ThemeVariant(value, ThemeVariant.Dark);
        //}

        /// <summary>
        /// Ignore this due to System.Text.Json serializer serializing the full CultureInfo object
        /// </summary>
        [JsonIgnore]
        public CultureInfo UiLanguage
        {
            get => _uiLanguage;
            set
            {
                _uiLanguage = value;
                Notify();
                UiLanguageChanged?.Invoke(this, new UiLanguageChangedEventArgs(_uiLanguage));
            }
        }

        [JsonPropertyName("ui_language")]
        public string UiLanguageString
        {
            get => _uiLanguage.ThreeLetterISOLanguageName;
            set => _uiLanguage = CultureInfo.GetCultureInfo(value);
        }

        [JsonPropertyName("collections")]
        public ObservableCollection<string>? CollectionFiles
        {
            get => _collectionFiles;
            set
            {
                _collectionFiles = value;
                Notify();
            }
        }
    }
}
