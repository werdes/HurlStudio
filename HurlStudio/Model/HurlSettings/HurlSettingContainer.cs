using Avalonia.Media;
using HurlStudio.Collections.Attributes;
using HurlStudio.Collections.Settings;
using HurlStudio.Common.UI;
using HurlStudio.Model.EventArgs;
using HurlStudio.UI.Controls.Documents;
using HurlStudio.UI.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Model.HurlSettings
{
    public class HurlSettingContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<SettingEnabledChangedEventArgs>? SettingEnabledChanged;
        public event EventHandler<SettingOrderChangedEventArgs>? SettingOrderChanged;
        public event EventHandler<SettingKeyChangedEventArgs>? SettingKeyChanged;

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

            _setting.PropertyChanged += On_Setting_PropertyChanged;
        }

        /// <summary>
        /// Listen for Property changes on the underlying setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Setting_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyInfo? propertyInfo = sender?.GetType().GetProperty(e.PropertyName ?? string.Empty);

            if (propertyInfo != null)
            {
                if(propertyInfo.CustomAttributes.Any(x => x.AttributeType == typeof(HurlSettingKeyAttribute)))
                {
                    this.SettingKeyChanged?.Invoke(this, new SettingKeyChangedEventArgs(this));
                }
            }
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


        public void MoveUp() => this.SettingOrderChanged?.Invoke(this, new SettingOrderChangedEventArgs(this, Enums.MoveDirection.Up));
        public void MoveDown() => this.SettingOrderChanged?.Invoke(this, new SettingOrderChangedEventArgs(this, Enums.MoveDirection.Down));

    }
}
