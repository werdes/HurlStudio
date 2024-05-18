using HurlStudio.UI.Localization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlSettings
{
    public class HurlSettingDocumentation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private List<Uri> _urls;
        private string? _description;
        private string? _shortDescription;
        private string? _localizedDescription;
        private string? _localizedShortDescription;

        public HurlSettingDocumentation()
        {
            _urls = new List<Uri>();

        }

        [JsonPropertyName("urls")]
        [ConfigurationKeyName("urls")]
        public List<Uri> Urls
        {
            get => _urls;
            set
            {
                _urls = value;
                this.Notify();
            }
        }

        [JsonPropertyName("description")]
        [ConfigurationKeyName("description")]
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                this.Notify();

                if (value != null)
                {
                    _localizedDescription = Localization.ResourceManager.GetString(value);
                    this.Notify(nameof(this.LocalizedDescription));
                }
            }
        }

        [JsonPropertyName("short_description")]
        [ConfigurationKeyName("short_description")]
        public string? ShortDescription
        {
            get => _shortDescription;
            set
            {
                _shortDescription = value;
                this.Notify();

                if (value != null)
                {
                    _localizedShortDescription = Localization.ResourceManager.GetString(value);
                    this.Notify(nameof(this.LocalizedShortDescription));
                }
            }
        }

        public string? LocalizedDescription
        {
            get => _localizedDescription;
        }

        public string? LocalizedShortDescription
        {
            get => _localizedShortDescription;
        }
    }
}
