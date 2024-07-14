using AvaloniaEdit.Document;
using HurlStudio.Model.EventArgs;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlFileTemplates
{
    public class HurlFileTemplateContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<HurlFileTemplateContainerSelectedEventArgs>? HurlFileTemplateContainerSelected;
        public event EventHandler<HurlFileTemplateContainerDeletedEventArgs>? HurlFileTemplateContainerDeleted;
        public event EventHandler<HurlFileTemplateContainerEditedEventArgs>? HurlFileTemplateContainerEdited;

        protected void Notify([CallerMemberName] string propertyName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private HurlFileTemplate _template;
        private HurlSettingSection _settingSection;
        private bool _isVisible;
        private TextDocument _document;

        public HurlFileTemplateContainer(HurlFileTemplate template, HurlSettingSection hurlSettingSection)
        {
            _template = template;
            _settingSection = hurlSettingSection;
            _isVisible = true;

            _document = new TextDocument(_template.Content);
        }

        public HurlFileTemplate Template
        {
            get => _template;
            set
            {
                _template = value;
                this.Notify();
            }
        }

        public HurlSettingSection SettingSection
        {
            get => _settingSection;
            set
            {
                _settingSection = value;
                this.Notify();
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                this.Notify();
            }
        }

        public TextDocument Document
        {
            get => _document;
        }

        public void NotifySelected() => this.HurlFileTemplateContainerSelected?.Invoke(this, new HurlFileTemplateContainerSelectedEventArgs(this));
        public void NotifyDeleted() => this.HurlFileTemplateContainerDeleted?.Invoke(this, new HurlFileTemplateContainerDeletedEventArgs(this));
        public void NotifyEdited() => this.HurlFileTemplateContainerEdited?.Invoke(this, new HurlFileTemplateContainerEditedEventArgs(this));
    }
}
