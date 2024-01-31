using Avalonia.Media;
using HurlStudio.Collections.Settings;
using HurlStudio.Model.EventArgs;
using HurlStudio.UI.Controls.Documents;
using HurlStudio.UI.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlSettings
{
    public class HurlSettingContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<SettingEnabledChangedEventArgs>? SettingEnabledChanged;

        protected void Notify([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _isReadOnly;
        private bool _collapsed;
        private bool _overwritten;
        private bool _enabled;
        private BaseSetting _setting;
        private FileDocumentViewModel _document;

        public HurlSettingContainer(FileDocumentViewModel document, BaseSetting setting, bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
            _collapsed = false;
            _enabled = true;
            _setting = setting;
            _document = document;
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                Notify();
            }
        }

        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                _collapsed = value;
                Notify();
            }
        }

        public bool Overwritten
        {
            get => _overwritten;
            set
            {
                _overwritten = value;
                Notify();
                Notify(nameof(DisplayOpacity));
                Notify(nameof(DisplayTextDecoration));
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                Notify();
                Notify(nameof(DisplayOpacity));
                SettingEnabledChanged?.Invoke(this, new SettingEnabledChangedEventArgs(_enabled));
            }
        }

        public BaseSetting Setting
        {
            get => _setting;
            set
            {
                _setting = value;
                Notify();
            }
        }

        public FileDocumentViewModel Document
        {
            get => _document;
        }

        public double DisplayOpacity
        {
            get => !_enabled || _overwritten ? 0.5D : 1D;
        }

        public TextDecorationCollection? DisplayTextDecoration
        {
            get => _overwritten ? TextDecorations.Strikethrough : null;
        }
    }
}
