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
            this._uiLanguage = defaultUiLanguage;
            this._collectionFiles = new ObservableCollection<string>();
            this._theme = defaultTheme;
        }

        /// <summary>
        /// Parameterless constructor for deserialization
        /// </summary>
        public UserSettings()
        {
            this._uiLanguage = CultureInfo.InvariantCulture;
            this._collectionFiles = new ObservableCollection<string>();
        }

        /// <summary>
        /// User selected theme variant
        /// Ignore this due to System.Text.Json serializer serializing the ThemeVariant record
        /// </summary>
        //[JsonIgnore]
        [JsonPropertyName("theme")]
        public ApplicationTheme Theme
        {
            get => this._theme;
            set
            {
                this._theme = value;
                Notify();
            }
        }

        //public string ThemeString
        //{
        //    get => this._theme.Key.ToString() ?? "Dark";
        //    set => this._theme = new ThemeVariant(value, ThemeVariant.Dark);
        //}

        /// <summary>
        /// Ignore this due to System.Text.Json serializer serializing the full CultureInfo object
        /// </summary>
        [JsonIgnore]
        public CultureInfo UiLanguage
        {
            get => this._uiLanguage;
            set
            {
                this._uiLanguage = value;
                Notify();
                UiLanguageChanged?.Invoke(this, new UiLanguageChangedEventArgs(this._uiLanguage));
            }
        }

        [JsonPropertyName("ui_language")]
        public string UiLanguageString
        {
            get => this._uiLanguage.ThreeLetterISOLanguageName;
            set => this._uiLanguage = CultureInfo.GetCultureInfo(value);
        }

        [JsonPropertyName("collections")]
        public ObservableCollection<string>? CollectionFiles
        {
            get => this._collectionFiles;
            set
            {
                this._collectionFiles = value;
                Notify();
            }
        }
    }
}
