using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace HurlStudio.Model.HurlFileTemplates
{
    public class HurlFileTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        private string? _name;
        private string? _content;
        private Guid _id;
        private List<string> _settings;
        private bool? _canDelete;
        private bool? _isDeleted;
        private bool? _isDefaultTemplate;
        
        public HurlFileTemplate()
        {
            _id = Guid.NewGuid();
            _settings = new List<string>();
        }
        
        [JsonPropertyName("name")]
        [ConfigurationKeyName("name")]
        public string? Name
        {
            get => _name;
            set
            {
                _name = value;  
                this.Notify();
            }
        }
        
        [JsonPropertyName("content")]
        [ConfigurationKeyName("content")]
        public string? Content
        {
            get => _content;
            set
            {
                _content = value;  
                this.Notify();
            }
        }

        [JsonPropertyName("id")]
        [ConfigurationKeyName("id")]
        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                this.Notify();
            }
        }

        [JsonPropertyName("settings")]
        [ConfigurationKeyName("settings")]
        public List<string> Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                this.Notify();
            }
        }

        [JsonPropertyName("can_delete")]
        [ConfigurationKeyName("canDelete")]
        public bool? CanDelete
        {
            get => _canDelete;
            set
            {
                _canDelete = value;
                this.Notify();
            }
        }

        [JsonPropertyName("is_deleted")]
        [ConfigurationKeyName("isDeleted")]
        public bool? IsDeleted
        {
            get => _isDeleted;
            set
            {
                _isDeleted = value;
                this.Notify();
            }
        }

        [JsonPropertyName("isDefaultTemplate")]
        [ConfigurationKeyName("is_default_template")]
        public bool? IsDefaultTemplate
        {
            get => _isDefaultTemplate;
            set
            {
                _isDefaultTemplate = value;
                this.Notify();
            }
        }
    }
}